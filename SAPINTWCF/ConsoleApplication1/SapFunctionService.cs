﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:2.0.50727.5466
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System.ServiceModel;

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="ISapFunctionService")]
public interface ISapFunctionService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISapFunctionService/ReadSAPTable", ReplyAction="http://tempuri.org/ISapFunctionService/ReadSAPTableResponse")]
    string ReadSAPTable(string systemName, string table);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public interface ISapFunctionServiceChannel : ISapFunctionService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public partial class SapFunctionServiceClient : System.ServiceModel.ClientBase<ISapFunctionService>, ISapFunctionService
{
    
    public SapFunctionServiceClient()
    {
    }
    
    public SapFunctionServiceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public SapFunctionServiceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public SapFunctionServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public SapFunctionServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public string ReadSAPTable(string systemName, string table)
    {
        return base.Channel.ReadSAPTable(systemName, table);
    }
}
