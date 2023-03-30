using System.ComponentModel;
using System.Reflection;

namespace TreatyAutomateSystem.Helpers;

public static class EnumHelpers
{
    public static string GetValueForTreatyFromDescription<T>(this T enumerationValue)
    where T : struct
    {
        Type type = enumerationValue.GetType();
        if (!type.IsEnum)
        {
            throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
        }

        //Tries to find a DescriptionAttribute for a potential friendly name
        //for the enum
        MemberInfo[]? memberInfo = type.GetMember(enumerationValue.ToString() ?? throw new NullReferenceException($"{enumerationValue} is null"));
        if (memberInfo != null && memberInfo.Length > 0)
        {
            object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attrs != null && attrs.Length > 0)
            {
                //Pull out the description value
                return ((DescriptionAttribute)attrs[0]).Description;
            }
        }
        //If we have no description attribute, just return the ToString of the enum
        throw new InvalidOperationException($"{enumerationValue} не содержит для {nameof(DescriptionAttribute)}атрибут для какого(их)-то значения(ий)");
    }

}
