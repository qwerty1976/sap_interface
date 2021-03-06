﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SAPINT.RFCTable;
using SAPINTDB;

namespace SAPINT.Idocs
{
    public class IdocDb
    {
        private String ConnectionName = null;
        private netlib7 logicDb = null;

        private DataTable idocHeader = null;
        private DataTable idocStatus = null;
        private DataTable idocItem = null;

        private Boolean appendTodb = false;

        public Boolean AppendTodb
        {
            get { return appendTodb; }
            set { appendTodb = value; }
        }

        public DataTable IdocHeader
        {
            get { return idocHeader; }
            set { idocHeader = value; }
        }

        public DataTable IdocItem
        {
            get { return idocItem; }
            set { idocItem = value; }
        }

        public DataTable IdocStatus
        {
            get { return idocStatus; }
            set { idocStatus = value; }
        }

        public IdocDb()
        {
            this.ConnectionName = ConfigFileTool.SAPGlobalSettings.GetDefaultDbConnection();
            logicDb = new netlib7(ConnectionName);

        }
        public IdocDb(String pConnectionName)
        {
            this.ConnectionName = pConnectionName;
            logicDb = new netlib7(ConnectionName);
        }
        /// <summary>
        /// 从本地数据读取IDOC数据，并解析成IDOC对象。
        /// </summary>
        /// <param name="pIdocNumber">IDOC编号</param>
        /// <param name="SystemName">此处的SAP系统名称用于本地数据库检索</param>
        /// <returns></returns>
        public Idoc ReadIdoc(string pIdocNumber, String SystemName)
        {
            try
            {
                idocHeader = new DataTable();
                idocItem = new DataTable();
                idocStatus = new DataTable();
                logicDb.DataTableFill(idocHeader, string.Format("select * from EDIDC where DOCNUM = '{0}' and SAPSYS = '{1}'", pIdocNumber, SystemName));
                if (!String.IsNullOrWhiteSpace(logicDb.ErrorMessage))
                {
                    throw new Exception(logicDb.ErrorMessage);
                }
                logicDb.DataTableFill(idocItem, string.Format("select * from EDID4 where DOCNUM = '{0}' and SAPSYS = '{1}'", pIdocNumber, SystemName));
                if (!String.IsNullOrWhiteSpace(logicDb.ErrorMessage))
                {
                    throw new Exception(logicDb.ErrorMessage);
                }
                logicDb.DataTableFill(idocStatus, string.Format("select * from EDIDS where DOCNUM = '{0}' and SAPSYS = '{1}'", pIdocNumber, SystemName));
                if (!String.IsNullOrWhiteSpace(logicDb.ErrorMessage))
                {
                    throw new Exception(logicDb.ErrorMessage);
                }

                if (!String.IsNullOrWhiteSpace(logicDb.ErrorMessage))
                {
                    throw new Exception(logicDb.ErrorMessage);
                }
                if (idocHeader != null && idocItem != null)
                {
                    SAPINT.Idocs.Meta.IdocUtil idocUtil = new SAPINT.Idocs.Meta.IdocUtil();
                    SAPINT.Idocs.Idoc idoc = idocUtil.ProcessSingleIdocFromDataTable(idocHeader, idocItem);
                    return idoc;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception exception)
            {

                throw new Exception(exception.Message);
            }

        }
        /// <summary>
        /// 根据IDOC编号把IDOC复制到本地。
        /// </summary>
        /// <param name="idocNumber">IDOC编号</param>
        /// <param name="SystemName">远程SAP系统名称</param>
        public void CopyIdocFromSAP(String idocNumber, String SystemName)
        {
            try
            {
                SAPINT.Utils.ReadTable idocReadItem = null;
                SAPINT.Utils.ReadTable idocReadHeader = null;
                SAPINT.Utils.ReadTable idocReadStatus = null;

                //DataTable dtIdocItem = new DataTable();
                //DataTable dtIdocHeder = new DataTable();
                //DataTable dtIdocStatus = new DataTable();

                idocNumber = idocNumber.TrimStart('0');
                String criteria = idocNumber.PadLeft(16, '0');
                criteria = String.Format("DOCNUM = '{0}'", criteria);

                String readTableFunction = ConfigFileTool.SAPGlobalSettings.GetReadTableFunction();

                idocReadItem = new SAPINT.Utils.ReadTable(SystemName);
                idocReadItem.TableName = "EDID4";
                idocReadItem.SetCustomFunctionName(readTableFunction);
                idocReadItem.AddCriteria(criteria);
                idocReadItem.Run();

                idocItem = idocReadItem.Result;

                if (idocItem.Rows.Count == 0)
                {
                    idocReadItem = new SAPINT.Utils.ReadTable(SystemName);
                    idocReadItem.TableName = "EDID2";
                    idocReadItem.SetCustomFunctionName(readTableFunction);
                    idocReadItem.AddCriteria(criteria);
                    idocReadItem.Run();
                    idocItem = idocReadItem.Result;

                }
                if (idocItem.Rows.Count == 0)
                {
                    idocReadItem = new SAPINT.Utils.ReadTable(SystemName);
                    idocReadItem.TableName = "EDIDD_OLD";
                    idocReadItem.SetCustomFunctionName(readTableFunction);
                    idocReadItem.AddCriteria(criteria);
                    idocReadItem.Run();
                    idocItem = idocReadItem.Result;
                }
                if (idocItem.Rows.Count == 0)
                {
                    throw new Exception(String.Format("无法找到IDOC{0}明细", idocNumber));
                }
                //读取IDOC头
                idocReadHeader = new SAPINT.Utils.ReadTable(SystemName);
                idocReadHeader.TableName = "EDIDC";
                idocReadHeader.SetCustomFunctionName(readTableFunction);
                idocReadHeader.AddCriteria(criteria);
                idocReadHeader.Run();
                idocHeader = idocReadHeader.Result;

                //if (idocHeader.Rows.Count != 1)
                //{
                //    throw new Exception(String.Format("无法找到IDOC{0}抬头定义", idocNumber));
                //}

                //读取IDOC状态
                idocReadStatus = new SAPINT.Utils.ReadTable(SystemName);
                idocReadStatus.TableName = "EDIDS";
                idocReadStatus.SetCustomFunctionName(readTableFunction);
                idocReadStatus.AddCriteria(criteria);
                idocReadStatus.Run();
                idocStatus = idocReadStatus.Result;



                SapTable idocTable = null;
                if (idocHeader.Rows.Count > 0)
                {
                    idocTable = new SapTable(SystemName, "EDIDC");
                    idocTable.DbConnectionString = this.ConnectionName;
                    idocTable.AppendToDb = AppendTodb;
                    idocTable.SaveDataTable(idocHeader);
                }


                if (idocItem.Rows.Count > 0)
                {
                    idocTable = new SapTable(SystemName, "EDID4", "EDID4");
                    idocTable.DbConnectionString = this.ConnectionName;
                    idocTable.AppendToDb = AppendTodb;
                    idocTable.SaveDataTable(idocItem);
                }


                if (idocStatus.Rows.Count > 0)
                {
                    idocTable = new SapTable(SystemName, "EDIDS");
                    idocTable.DbConnectionString = this.ConnectionName;
                    idocTable.AppendToDb = AppendTodb;
                    idocTable.SaveDataTable(idocStatus);

                }

            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
