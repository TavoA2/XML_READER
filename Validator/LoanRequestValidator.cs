using SGM_WEBAPI.Models;
using static SGM_WEBAPI.Models.LoanRequest;

namespace SGM_WEBAPI.Validator { 
public class LoanValidator
{
    public bool Validate(LoanRequest loanRequest, out string errorMessage)
    {
        LoanRequest loan = new LoanRequest();

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

        // Realizar otras validaciones
        if (!ValidateDateOfBirth(loanRequest.DateOfBirth))
        {
            errorMessage = "La persona debe ser mayor de 18 años.";
            return false;
        }

        if (!ValidateEmployerLength(loanRequest.EmployerLengthMonths))
        {
            errorMessage = "La longitud del empleo debe ser mayor o igual a 12 meses.";
            return false;
        }

        
        if (!ValidateLoanAmount(loanRequest.LoanAmountDesired))
        {
            errorMessage = "El monto del préstamo debe estar entre $200 y $500.";
            return false;
        }

        if (!ValidateHomeState(loanRequest.HomeState))
        {
            errorMessage = "El estado de residencia es inválido.";
            return false;
        }

        if (!ValidateIncomeFrequency(loanRequest.IncomeFrequency))
        {
            errorMessage = "La frecuencia de ingresos es inválida.";
            return false;
        }

        if (!ValidateIncomeAmount(loanRequest.IncomeAmount, loanRequest.IncomeFrequency))
        {
            errorMessage = "El monto del último trabajo no cumple con las validaciones.";
            return false;
        }

        if (!ValidateMilitaryStatus(loanRequest.Military))
        {
            errorMessage = "El estado militar debe ser falso.";
            return false;
        }

        return true;
    }

    private bool ValidateDateOfBirth(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;

        return age >= 18;
    }

    private bool ValidateEmployerLength(int employerLengthMonths)
    {
        return employerLengthMonths >= 12;
    }

    private bool ValidateLoanAmount(decimal loanAmountDesired)
    {
        return loanAmountDesired >= 200 && loanAmountDesired <= 500;
    }

    private bool ValidateHomeState(string homeState)
    {
        return homeState.Length == 2 && !ProhibitedStates.Contains(homeState);
    }

    private bool ValidateIncomeFrequency(string incomeFrequency)
    {
        var validFrequencies = new List<string> { "Weekly", "Biweekly", "Monthly", "Twice" };
        return validFrequencies.Contains(incomeFrequency);
    }

    private bool ValidateIncomeAmount(List<IncomeItem> incomeItems, string incomeFrequency)
    {
        if (incomeFrequency == "Weekly" || incomeFrequency == "Biweekly" || incomeFrequency == "Twice")
        {
            var lastSalary = incomeItems?.LastOrDefault()?.Salary ?? 0;
            var monthlySalary = CalculateMonthlySalary(lastSalary, incomeFrequency);
            return monthlySalary >= 0.85m * 2000 && monthlySalary <= 1.15m * 2000;
        }

        return incomeItems?.LastOrDefault()?.Salary >= 0.85m * 2000 && incomeItems.LastOrDefault()?.Salary <= 1.15m * 2000;
    }

    private bool ValidateMilitaryStatus(bool military)
    {
        return !military;
    }

    private decimal CalculateMonthlySalary(decimal salary, string incomeFrequency)
    {
        switch (incomeFrequency)
        {
            case "Weekly":
                return salary * 4;
            case "Biweekly":
            case "Twice":
                return salary * 2;
            case "Monthly":
                return salary;
            default:
                return 0;
        }
    }

    private static readonly List<string> ProhibitedStates = new List<string> { "DE", "CA", "CO", "FL", "NY", "ND", "TX", "SC" };
}
}
