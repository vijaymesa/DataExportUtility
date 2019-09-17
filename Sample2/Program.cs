using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample2
{
    class Program
    {
        private static string BASEURL = "http://dummy.restapiexample.com/api/v1/";
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                var response = GetRestClientResponse(Method.GET, "employees");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var employeeList = JsonConvert.DeserializeObject<List<EmployeeModel>>(response.Content);

                    string result = BindingData(employeeList);

                    CreateJsonData(result); //Creates Json File
                }
                else
                    _logger.Error(response.Content);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace));
            }
            
            Console.ReadLine();
        }

        private static IRestResponse GetRestClientResponse(Method method, string restApiUrl, object requestObj = null, string accessToken = null)
        {
            try
            {
                var restClientAPIUrl = BASEURL + restApiUrl;
                var restClient = new RestClient(restClientAPIUrl);
                var request = new RestRequest(method) { RequestFormat = DataFormat.Json };

                if (!string.IsNullOrEmpty(accessToken)) //Checking if we have access token
                    request.AddHeader("Authorization", "Bearer " + accessToken);

                var clientResponse = restClient.Execute(request);
                return clientResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        private static string BindingData(List<EmployeeModel> employeeList)
        {
            var empObj = employeeList.Select(x => new EmpModal
            {
                EmployeeAge = x.employee_age,
                EmployeeName = x.employee_name,
                EmployeeSalary = x.employee_salary,
                Id = x.id,
                ProfileImage = x.profile_image,
                DataRetrievedOn = string.Format("{0: dd MMM yyyy}", DateTime.UtcNow),
                DOB = string.Format("{0: dd MMM yyyy}", DateTime.UtcNow)
            }).ToList();

            return JsonConvert.SerializeObject(empObj);
        }

        private static void CreateJsonData(string result)
        {
            string path = "emplyoee.json";

            if (File.Exists(path))
                File.Delete(path);

            using (var sw = new StreamWriter(path, true))
            {
                sw.WriteLine(result.ToString());
                sw.Close();
            }
        }
    }
}
