using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace EmployeeDataManipulation
{

    public class EmployeeRepo : IEmployeeRepo
    {
        private readonly IAmazonDynamoDB dynamoDB;
        private readonly ILambdaContext context;

        public EmployeeRepo(IAmazonDynamoDB dynamoDB, ILambdaContext context)
        {
            this.dynamoDB = dynamoDB;
            this.context = context;
        }

        public async Task<bool> CreateEmployeeAsync(Employee emp)
        {
            var request = new PutItemRequest
            {
                TableName = "Employee",
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Emp_ID",new AttributeValue(emp.Emp_ID) },
                    { "Emp_name",new AttributeValue(emp.Emp_name) },
                    { "Emp_location",new AttributeValue(emp.Emp_location) }
                }
            };
            context.Logger.Log(JsonConvert.SerializeObject(request));
            var response = await dynamoDB.PutItemAsync(request);
            context.Logger.Log(JsonConvert.SerializeObject(response));
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> UpdateEmployeeAsync(Employee emp)
        {
            try
            {
                var request = new UpdateItemRequest
                {
                    TableName = "Employee",
                    Key = new Dictionary<string, AttributeValue>
                {
                    {"Emp_ID", new AttributeValue{S =emp.Emp_ID} },
                    {"Emp_name", new AttributeValue{S =emp.Emp_name } }
                },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":loc",new AttributeValue{S = emp.Emp_location} }
                    //{":sal", new AttributeValue{N = "100090"} }
                },
                    UpdateExpression = "SET Emp_location = :loc",
                    ReturnValues = "ALL_NEW"
                };

                var response = await dynamoDB.UpdateItemAsync(request);
                context.Logger.Log(JsonConvert.SerializeObject(response));
                return true;
            }
            catch (Exception ex)
            {

                context.Logger.Log(ex.Message);
            }
            

            return false;
        }

        public async Task<bool> DeleteEmployeeAsync(Employee emp)
        {
            try
            {
                var request = new DeleteItemRequest
                {
                    TableName = "Employee",
                    Key = new Dictionary<string, AttributeValue>
                    {
                        {"Emp_ID", new AttributeValue{S =emp.Emp_ID} },
                        {"Emp_name", new AttributeValue{ S=emp.Emp_name} }
                    }
                };
                var response = await dynamoDB.DeleteItemAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                context.Logger.Log(ex.Message);
                return false;
            }
        }
    }
}
