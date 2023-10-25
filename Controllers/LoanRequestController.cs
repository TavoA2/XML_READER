using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Xml.Linq;
using SGM_WEBAPI.Models;
using SGM_WEBAPI.Validator;


namespace SGM_WEBAPI.Controllers
{
    [Route("api/loanrequest")]
    public class LoanRequestController : ControllerBase
    {

        [HttpPost]
        public async Task<ActionResult> ProcessLoanRequest()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    string xmlData = await reader.ReadToEndAsync();
                    var loanRequest = DeserializeLoanRequest(xmlData);

                    //prueba
                    Console.WriteLine($"XML Recibido:\n{xmlData}");
                    var serializer = new XmlSerializer(typeof(LoanRequest));
                    using (var reader1 = new StringReader(xmlData))
                    {
                        var loanRequest1 = (LoanRequest)serializer.Deserialize(reader);
                        Console.WriteLine($"LoanRequest Deserializado:\n{JsonConvert.SerializeObject(loanRequest1, Formatting.Indented)}");
                    }

                    Console.WriteLine($"LoanRequest Deserializado:\n{JsonConvert.SerializeObject(loanRequest, Formatting.Indented)}");
                    //fin de prueba 


                    //Se comenta las validaciones debido al error que comento en README.md, que no pudo acabar debido a la limitacion de tiempo. 
                    var loanValidator = new LoanValidator(); 
                    if (!loanValidator.Validate(loanRequest, out string errorMessage))
                    {
                        return BadRequest($"{{ \"message\": \"El XML no cumple con el formato válido. {errorMessage}\" }}");
                    }

                    if (!Validate(loanRequest, out string errorMessage2))
                    {
                        return BadRequest($"{{ \"message\": \"El XML no cumple con el formato válido. {errorMessage2}\" }}");
                    }

                    // Obtener información del XML según sea necesario
                    string customerId = loanRequest.DataItems.First(d => d.Name == "customer_id").Value;
                    string firstName = loanRequest.DataItems.First(d => d.Name == "name_first").Value;
                    string lastName = loanRequest.DataItems.First(d => d.Name == "name_last").Value;
                    string lastJobDate = GetLastJobDate(loanRequest);

                    // Retornar respuesta exitosa
                    return Ok($"{{ \"message\": \"El XML ID: {customerId}, fue aceptado con el nombre {firstName} {lastName} y su último trabajo fue el {lastJobDate}\" }}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ProcessLoanRequest: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Error interno: " + ex.Message });
            }
        }

        private bool Validate(LoanRequest loanRequest, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Verificar si loanRequest es nulo
            if (loanRequest == null)
            {
                errorMessage = "El objeto LoanRequest es nulo.";
                return false;
            }

            // Verificar si DataItems es nulo o vacío
            if (loanRequest.DataItems == null || !loanRequest.DataItems.Any())
            {
                errorMessage = "La propiedad DataItems de LoanRequest es nula o vacía.";
                return false;
            }

            // Obtener el valor de 'customer_id' de DataItems
            var customerIdDataItem = loanRequest.DataItems.FirstOrDefault(d => d.Name == "customer_id");

            // Verificar si customerIdDataItem es nulo
            if (customerIdDataItem == null)
            {
                errorMessage = "No se encontró el elemento 'customer_id' en DataItems.";
                return false;
            }

            // Obtener el valor real de 'customer_id'
            var customerId = customerIdDataItem.Value;

            // Verificar si customerId es nulo o vacío
            if (string.IsNullOrEmpty(customerId))
            {
                errorMessage = "El valor de 'customer_id' en DataItems es nulo o vacío.";
                return false;
            }
            return true;
        }

        private string GetLastJobDate(LoanRequest loanRequest)
        {
            try
            {
                if (loanRequest.IncomeAmount == null || !loanRequest.IncomeAmount.Any())
                {
                    return "No hay trabajos registrados";
                }

                // Obtener las fechas no nulas y ordenarlas
                var validDates = loanRequest.IncomeAmount
                    .Where(item => !string.IsNullOrEmpty(item.LastDate))
                    .OrderByDescending(item => DateTime.TryParse(item.LastDate, out var date) ? date : DateTime.MinValue);

                if (!validDates.Any())
                {
                    return "No hay fechas válidas";
                }

                // Tomar la fecha del primer trabajo (el más reciente)
                DateTime lastJobDate = DateTime.Parse(validDates.First().LastDate);

                // Formatear la fecha según sea necesario
                string formattedDate = lastJobDate.ToString("dd/MM/yyyy");

                return formattedDate;
            }
            catch (Exception ex)
            {
                // Loguear la excepción para depuración
                Console.WriteLine($"Error en GetLastJobDate: {ex.Message}");
                return "Fecha no disponible (Error interno)";
            }
        }

        public static LoanRequest DeserializeLoanRequest(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                throw new ArgumentException("El XML proporcionado está vacío o es nulo.");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(LoanRequest));

            using (StringReader reader = new StringReader(xmlString))
            {
                LoanRequest loanRequest = (LoanRequest)serializer.Deserialize(reader);

                // Deserializar income_amount como lista de objetos IncomeItem
                XDocument xDoc = XDocument.Parse(xmlString);
                XElement incomeAmountElement = xDoc.Descendants("data").FirstOrDefault(e => e.Attribute("name")?.Value == "income_amount");

                if (incomeAmountElement != null)
                {
                    Console.WriteLine($"incomeAmountElement.Value: {incomeAmountElement.Value}");
                    //string incomeAmountJson = incomeAmountElement.Value;
                    loanRequest.IncomeAmount = JsonConvert.DeserializeObject<List<LoanRequest.IncomeItem>>(incomeAmountElement.Value);
                    //loanRequest.IncomeAmount = JsonConvert.DeserializeObject<List<LoanRequest.IncomeItem>>(incomeAmountJson);
                }
                else
                {
                    Console.WriteLine("incomeAmountElement is null");
                }
                return loanRequest;
            }
        }
    }
}

