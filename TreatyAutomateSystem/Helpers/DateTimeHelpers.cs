namespace TreatyAutomateSystem.Helpers;

public static class DateTimeHelpers
{
    public static DateTime ParseFromOADateOrString(this string date)
    {
        double dDateParsed;

        double.TryParse(date, out dDateParsed);

        if(double.TryParse(date, out dDateParsed))
            return FromOADateWihAppException(dDateParsed);
        
        DateTime res;

        if(!DateTime.TryParse(date, out res))
            throw new AppExceptionBase($"Неправильный формат даты {date}");
        return res;

    }
    static DateTime FromOADateWihAppException(double dDate)
    {
        try
        {
            return DateTime.FromOADate(dDate);
        }
        catch(ArgumentException)
        {
            throw new AppExceptionBase($"Неправильный формат даты {dDate}");
        }
    }

    
}