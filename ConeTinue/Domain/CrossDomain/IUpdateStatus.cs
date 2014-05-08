using System;
using System.IO;
using System.ServiceModel;

namespace ConeTinue.Domain.CrossDomain
{
	[ServiceContract]
	public interface IUpdateStatus
	{
		[OperationContract]
		void Update(TestKey testKey, TestStatus status);

		[OperationContract]
		void UpdateTestTime(TestKey testKey, TimeSpan time);

		[OperationContract]
		void Failed(TestFailure failure);

		[OperationContract]
		void SetStatus(string message);

		[OperationContract]
		void ReportError(string error, string details);

		[OperationContract]
		void ReportInfo(string info);
	}
}