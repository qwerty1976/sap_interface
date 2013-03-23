﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAP.Middleware.Connector;
using System.Collections;
using System.Data;

namespace SAPINT.Idocs.Meta
{
    public struct EDI_IAPI12
    {
        public System.String SEGMENTTYP { get; set; } // 段类型（30 字符格式）
        public System.String FIELDNAME { get; set; } // 字段名
        public System.Int32 INTLEN { get; set; } // 以字节计的内部长度
        public System.Int32 EXTLEN { get; set; } // 输出长度
        public System.Int32 FIELD_POS { get; set; } // 字段的位置号码
        public System.Int32 BYTE_FIRST { get; set; } // 第一个字节的位置
        public System.Int32 BYTE_LAST { get; set; } // 最后一个字节的位置
        public System.String ROLLNAME { get; set; } // 数据元素 (语义域)
        public System.String DOMNAME { get; set; } // 定义域名
        public System.String DATATYPE { get; set; } // ABAP/4 字典: 屏幕绘制器的屏幕数据类型
        public System.String DESCRP { get; set; } // 对象的简短说明
        public System.String ISOCODE { get; set; } // IDoc 开发：字段中的 ISO 代码标识
        public System.String VALUETAB { get; set; } // IDoc 段字段的值表
    }

    public struct EDI_IAPI11
    {
        public System.Int32 NR { get; set; } // IDoc 类型中段的序列号
        public System.String SEGMENTTYP { get; set; } // 段类型（30 字符格式）
        public System.String SEGMENTDEF { get; set; } // IDoc 开发：段定义
        public System.String QUALIFIER { get; set; } // 标记：IDoc 中限定的段
        public System.Int32 SEGLEN { get; set; } // 一个字段的长度(位置的数目)
        public System.String PARSEG { get; set; } // 段类型（30 字符格式）
        public System.Int32 PARPNO { get; set; } // 父代段的序列号
        public System.String PARFLG { get; set; } // 父段标记：段是段组的开始
        public System.String MUSTFL { get; set; } // 标记：强制条目
        public System.Int32 OCCMIN { get; set; } // 序列中段的最小数目
        public System.Double OCCMAX { get; set; } // 序列中最大段数目
        public System.Int32 HLEVEL { get; set; } // IDoc 类型段的层次水平
        public System.String DESCRP { get; set; } // 对象的简短说明
        public System.String GRP_MUSTFL { get; set; } // 组标记：强制
        public System.Int32 GRP_OCCMIN { get; set; } // 序列中最小组号
        public System.Double GRP_OCCMAX { get; set; } // 序列中最大组号
        public System.String REFSEGTYP { get; set; } // 段类型（30 字符格式）
    }

    public struct EDI_IAPI14
    {
        public System.String STRNAME { get; set; } // 内部结构的名称
        public System.String FIELDNAME { get; set; } // 字段名
        public System.String FLDVALUE_L { get; set; } // 下限值 / 单一值
        public System.String FLDVALUE_H { get; set; } // 上限值
        public System.String DESCRP { get; set; } // 说明简要文字
    }

    public struct EDI_IAPI17
    {
        public System.String MESTYP { get; set; } // 消息类型
        public System.String DESCRP { get; set; } // 对象的简短说明
        public System.String IDOCTYP { get; set; } // 基本类型
        public System.String CIMTYP { get; set; } // 分机号
        public System.String RELEASED { get; set; } // 消息类型分配有效的版本
    }

    /// <summary>
    /// 使用SAP服务器来解析一个IDOC。
    /// </summary>
    public class IdocMeta
    {

        private Idoc idoc = null;
        private String sysName;

        private List<EDI_IAPI12> _FIELDS = null;
        private List<EDI_IAPI14> _FVALUES = null;
        private List<EDI_IAPI17> _MESSAGES = null;
        private List<EDI_IAPI11> _SEGMENTS = null;

        private List<Dictionary<String, String>> IdocAsFlatList = null;

        //public Idoc Idoc
        //{
        //    get { return idoc; }
        //    set
        //    {
        //        idoc = value;
        //       // getIdocTypeDefinition();

        //    }
        //}

        public String SystemName
        {
            get { return sysName; }
            set { sysName = value; }

        }
        public List<Dictionary<String, String>> IdocValueAsList()
        {
            return IdocAsFlatList;
        }

        public IdocMeta(String psystemName, Idoc pidoc)
        {
            this.sysName = psystemName;
            this.idoc = pidoc;

            _FIELDS = new List<EDI_IAPI12>();
            _FVALUES = new List<EDI_IAPI14>();
            _MESSAGES = new List<EDI_IAPI17>();
            _SEGMENTS = new List<EDI_IAPI11>();
            IdocAsFlatList = new List<Dictionary<string, string>>();

        }
        
        private void deCompileSegment(IdocSegment segment)
        {

            string type = null;
            // segment.SegmentType
            if (_SEGMENTS.Exists(seg => seg.SEGMENTTYP == segment.SegmentName))
            {
                var ls = from segs in _SEGMENTS
                         where segs.SEGMENTTYP == segment.SegmentName
                         select segs.SEGMENTTYP;
                type = ls.First();

            }
            else if (_SEGMENTS.Exists(seg => seg.SEGMENTDEF == segment.SegmentName))
            {
                var lss = from segs in _SEGMENTS
                          where segs.SEGMENTDEF == segment.SegmentName
                          select segs.SEGMENTTYP;
                type = lss.First();
            }
            if (type != null)
            {
                var fields = from field_list in _FIELDS
                             where field_list.SEGMENTTYP == type
                             select field_list;


                int offset = 0;
                int lenght = 0;
                String tmpField = null;
                Dictionary<String, String> onerow = new Dictionary<string, string>();

                foreach (var item in fields)
                {

                    lenght = item.EXTLEN;
                    tmpField = segment.ReadDataBuffer(offset, lenght);
                    tmpField = tmpField.PadLeft(lenght, '0');
                    tmpField = tmpField.Trim();
                    onerow.Add(item.FIELDNAME, tmpField);
                    segment.Fields.Add(item.FIELDNAME, item.DESCRP, item.EXTLEN, offset, item.DATATYPE, tmpField);
                    offset += item.EXTLEN;
                }
                IdocAsFlatList.Add(onerow);
            }
            if (segment.HasChildren)
            {
                for (int i = 0; i < segment.ChildSegments.Count; i++)
                {
                    deCompileSegment(segment.ChildSegments[i]);
                }
            }


        }

        public Idoc GetNewIdoc()
        {
           // this.idoc = poldIdoc;
            getIdocTypeDefinition();

            if (idoc == null)
            {
                throw new SAPException("Idoc is null");
            }
            for (int i = 0; i < idoc.Segments.Count; i++)
            {
                IdocSegment segment = idoc.Segments[i];
                deCompileSegment(segment);
            }
            return idoc;
        }

        /// <summary>
        /// 根据IDOC的IDOC类型，到SAP系统里查找它对应的类型定义
        /// </summary>
        private void getIdocTypeDefinition()
        {
            if (idoc==null)
            {
                throw new SAPException("IDOC是空值。");
            }
            if (idoc != null)
            {
                if (String.IsNullOrWhiteSpace(idoc.IDOCTYP))
                {
                    throw new SAPException("空白的IDOC类型");
                }
                RfcDestination destination = SAPDestination.GetDesByName(sysName);

                IRfcFunction function = destination.Repository.CreateFunction("IDOCTYPE_READ_COMPLETE");
                function.SetValue("PI_IDOCTYP", idoc.IDOCTYP);
                function.SetValue("PI_CIMTYP", idoc.CIMTYP);
                function.Invoke(destination);
                IRfcTable rfctable_PT_FIELDS = function.GetTable("PT_FIELDS");

                EDI_IAPI12 _EDI_IAPI12;
                for (int i = 0; i < rfctable_PT_FIELDS.RowCount; i++)
                {
                    _EDI_IAPI12 = new EDI_IAPI12();
                    _EDI_IAPI12.SEGMENTTYP = rfctable_PT_FIELDS[i].GetString("SEGMENTTYP"); // 段类型（30 字符格式）
                    _EDI_IAPI12.FIELDNAME = rfctable_PT_FIELDS[i].GetString("FIELDNAME"); // 字段名
                    _EDI_IAPI12.INTLEN = rfctable_PT_FIELDS[i].GetInt("INTLEN"); // 以字节计的内部长度
                    _EDI_IAPI12.EXTLEN = rfctable_PT_FIELDS[i].GetInt("EXTLEN"); // 输出长度
                    _EDI_IAPI12.FIELD_POS = rfctable_PT_FIELDS[i].GetInt("FIELD_POS"); // 字段的位置号码
                    _EDI_IAPI12.BYTE_FIRST = rfctable_PT_FIELDS[i].GetInt("BYTE_FIRST"); // 第一个字节的位置
                    _EDI_IAPI12.BYTE_LAST = rfctable_PT_FIELDS[i].GetInt("BYTE_LAST"); // 最后一个字节的位置
                    _EDI_IAPI12.ROLLNAME = rfctable_PT_FIELDS[i].GetString("ROLLNAME"); // 数据元素 (语义域)
                    _EDI_IAPI12.DOMNAME = rfctable_PT_FIELDS[i].GetString("DOMNAME"); // 定义域名
                    _EDI_IAPI12.DATATYPE = rfctable_PT_FIELDS[i].GetString("DATATYPE"); // ABAP/4 字典: 屏幕绘制器的屏幕数据类型
                    _EDI_IAPI12.DESCRP = rfctable_PT_FIELDS[i].GetString("DESCRP"); // 对象的简短说明
                    _EDI_IAPI12.ISOCODE = rfctable_PT_FIELDS[i].GetString("ISOCODE"); // IDoc 开发：字段中的 ISO 代码标识
                    _EDI_IAPI12.VALUETAB = rfctable_PT_FIELDS[i].GetString("VALUETAB"); // IDoc 段字段的值表
                    _FIELDS.Add(_EDI_IAPI12);

                }

                IRfcTable rfctable_PT_FVALUES = function.GetTable("PT_FVALUES");

                EDI_IAPI14 _EDI_IAPI14;
                for (int i = 0; i < rfctable_PT_FVALUES.RowCount; i++)
                {
                    _EDI_IAPI14 = new EDI_IAPI14();
                    _EDI_IAPI14.STRNAME = rfctable_PT_FVALUES[i].GetString("STRNAME"); // 内部结构的名称
                    _EDI_IAPI14.FIELDNAME = rfctable_PT_FVALUES[i].GetString("FIELDNAME"); // 字段名
                    _EDI_IAPI14.FLDVALUE_L = rfctable_PT_FVALUES[i].GetString("FLDVALUE_L"); // 下限值 / 单一值
                    _EDI_IAPI14.FLDVALUE_H = rfctable_PT_FVALUES[i].GetString("FLDVALUE_H"); // 上限值
                    _EDI_IAPI14.DESCRP = rfctable_PT_FVALUES[i].GetString("DESCRP"); // 说明简要文字
                    _FVALUES.Add(_EDI_IAPI14);
                }

                IRfcTable rfctable_PT_MESSAGES = function.GetTable("PT_MESSAGES");

                EDI_IAPI17 _EDI_IAPI17;
                for (int i = 0; i < rfctable_PT_MESSAGES.RowCount; i++)
                {
                    _EDI_IAPI17 = new EDI_IAPI17();
                    _EDI_IAPI17.MESTYP = rfctable_PT_MESSAGES[i].GetString("MESTYP"); // 消息类型
                    _EDI_IAPI17.DESCRP = rfctable_PT_MESSAGES[i].GetString("DESCRP"); // 对象的简短说明
                    _EDI_IAPI17.IDOCTYP = rfctable_PT_MESSAGES[i].GetString("IDOCTYP"); // 基本类型
                    _EDI_IAPI17.CIMTYP = rfctable_PT_MESSAGES[i].GetString("CIMTYP"); // 分机号
                    _EDI_IAPI17.RELEASED = rfctable_PT_MESSAGES[i].GetString("RELEASED"); // 消息类型分配有效的版本
                    _MESSAGES.Add(_EDI_IAPI17);
                }

                IRfcTable rfctable_PT_SEGMENTS = function.GetTable("PT_SEGMENTS");

                EDI_IAPI11 _EDI_IAPI11;
                for (int i = 0; i < rfctable_PT_SEGMENTS.RowCount; i++)
                {
                    _EDI_IAPI11 = new EDI_IAPI11();
                    _EDI_IAPI11.NR = rfctable_PT_SEGMENTS[i].GetInt("NR"); // IDoc 类型中段的序列号
                    _EDI_IAPI11.SEGMENTTYP = rfctable_PT_SEGMENTS[i].GetString("SEGMENTTYP"); // 段类型（30 字符格式）
                    _EDI_IAPI11.SEGMENTDEF = rfctable_PT_SEGMENTS[i].GetString("SEGMENTDEF"); // IDoc 开发：段定义
                    _EDI_IAPI11.QUALIFIER = rfctable_PT_SEGMENTS[i].GetString("QUALIFIER"); // 标记：IDoc 中限定的段
                    _EDI_IAPI11.SEGLEN = rfctable_PT_SEGMENTS[i].GetInt("SEGLEN"); // 一个字段的长度(位置的数目)
                    _EDI_IAPI11.PARSEG = rfctable_PT_SEGMENTS[i].GetString("PARSEG"); // 段类型（30 字符格式）
                    _EDI_IAPI11.PARPNO = rfctable_PT_SEGMENTS[i].GetInt("PARPNO"); // 父代段的序列号
                    _EDI_IAPI11.PARFLG = rfctable_PT_SEGMENTS[i].GetString("PARFLG"); // 父段标记：段是段组的开始
                    _EDI_IAPI11.MUSTFL = rfctable_PT_SEGMENTS[i].GetString("MUSTFL"); // 标记：强制条目
                    _EDI_IAPI11.OCCMIN = rfctable_PT_SEGMENTS[i].GetInt("OCCMIN"); // 序列中段的最小数目
                    _EDI_IAPI11.OCCMAX = rfctable_PT_SEGMENTS[i].GetDouble("OCCMAX"); // 序列中最大段数目
                    _EDI_IAPI11.HLEVEL = rfctable_PT_SEGMENTS[i].GetInt("HLEVEL"); // IDoc 类型段的层次水平
                    _EDI_IAPI11.DESCRP = rfctable_PT_SEGMENTS[i].GetString("DESCRP"); // 对象的简短说明
                    _EDI_IAPI11.GRP_MUSTFL = rfctable_PT_SEGMENTS[i].GetString("GRP_MUSTFL"); // 组标记：强制
                    _EDI_IAPI11.GRP_OCCMIN = rfctable_PT_SEGMENTS[i].GetInt("GRP_OCCMIN"); // 序列中最小组号
                    _EDI_IAPI11.GRP_OCCMAX = rfctable_PT_SEGMENTS[i].GetDouble("GRP_OCCMAX"); // 序列中最大组号
                    _EDI_IAPI11.REFSEGTYP = rfctable_PT_SEGMENTS[i].GetString("REFSEGTYP"); // 段类型（30 字符格式）
                    _SEGMENTS.Add(_EDI_IAPI11);
                }
            }
        }
    }
}