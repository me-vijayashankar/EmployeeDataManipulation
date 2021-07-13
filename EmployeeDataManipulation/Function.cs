using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EmployeeDataManipulation
{
    public class Function
    {
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                var respHeader = new Dictionary<string, string>();
                respHeader.Add("Access-Control-Allow-Origin", "*");
                if (request.HttpMethod == "POST")
                {                    
                    var employee = JsonConvert.DeserializeObject<Employee>(request.Body);
                    if (employee == null) return new APIGatewayProxyResponse { StatusCode = (int)System.Net.HttpStatusCode.BadRequest };
                    context.Logger.Log(JsonConvert.SerializeObject(employee));
                    IEmployeeRepo repo = new EmployeeRepo(new AmazonDynamoDBClient(), context);
                    var status = await repo.CreateEmployeeAsync(employee);
                    if (status)
                        return new APIGatewayProxyResponse { StatusCode = (int)System.Net.HttpStatusCode.Created, Body = "Created Successfully",Headers= respHeader };
                    else
                        return new APIGatewayProxyResponse { StatusCode = (int)System.Net.HttpStatusCode.BadRequest, Headers = respHeader };
                }
                else if(request.HttpMethod == "PUT")
                {
                    var employee = JsonConvert.DeserializeObject<Employee>(request.Body);
                    if (employee == null) return new APIGatewayProxyResponse { StatusCode = (int)System.Net.HttpStatusCode.BadRequest, Headers = respHeader };
                    context.Logger.Log(JsonConvert.SerializeObject(employee));
                    IEmployeeRepo repo = new EmployeeRepo(new AmazonDynamoDBClient(), context);
                    if(await repo.UpdateEmployeeAsync(employee))
                        return new APIGatewayProxyResponse { StatusCode = (int)System.Net.HttpStatusCode.NoContent, Body = "Updted Successfully", Headers = respHeader };
                    else return new APIGatewayProxyResponse { StatusCode = (int)System.Net.HttpStatusCode.BadRequest, Headers = respHeader };

                }
                else if (request.HttpMethod == "DELETE" && request.Resource == "/employee/{id}")
                {
                    IEmployeeRepo repo = new EmployeeRepo(new AmazonDynamoDBClient(), context);
                    var empid = request.PathParameters["id"];
                    var employee = JsonConvert.DeserializeObject<Employee>(request.Body);

                    if (await repo.DeleteEmployeeAsync(employee))
                    {
                        return new APIGatewayProxyResponse { StatusCode = (int)System.Net.HttpStatusCode.NoContent, Body = "Deleted Successfully", Headers = respHeader };
                    }
                    context.Logger.Log(JsonConvert.SerializeObject(request));
                    return new APIGatewayProxyResponse { StatusCode = (int)System.Net.HttpStatusCode.BadRequest, Headers = respHeader };
                }
                else if (request.HttpMethod == "DELETE" && request.Resource == "/employee"){
                    IEmployeeRepo repo = new EmployeeRepo(new AmazonDynamoDBClient(), context);
                    var employee = JsonConvert.DeserializeObject<Employee>(request.Body);

                    if (await repo.DeleteEmployeeAsync(employee))
                    {
                        return new APIGatewayProxyResponse { StatusCode = (int)System.Net.HttpStatusCode.NoContent, Body = "Deleted Successfully", Headers = respHeader };
                    }
                    context.Logger.Log(JsonConvert.SerializeObject(request));
                    return new APIGatewayProxyResponse { StatusCode = (int)System.Net.HttpStatusCode.BadRequest, Headers = respHeader };
                }
                else return new APIGatewayProxyResponse { StatusCode = (int)System.Net.HttpStatusCode.BadRequest, Headers = respHeader };

            }
            catch (Exception ex)
            {
                return new APIGatewayProxyResponse { StatusCode = 500, Body = ex.Message };
            }           
            
        }
    }
}
