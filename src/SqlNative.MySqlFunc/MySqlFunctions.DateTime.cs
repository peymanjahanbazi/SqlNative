namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Returns the current date and time.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT NOW(); -&gt; '2024-01-15 14:30:00'
    /// </code>
    /// </example>
    public static string NOW()
    {
        return "NOW()";
    }

    /// <summary>
    /// Returns the current date.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CURDATE(); -&gt; '2024-01-15'
    /// </code>
    /// </example>
    public static string CURDATE()
    {
        return "CURDATE()";
    }

    /// <summary>
    /// Synonym for <see cref="CURDATE"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CURRENT_DATE(); -&gt; '2024-01-15'
    /// </code>
    /// </example>
    public static string CURRENT_DATE()
    {
        return "CURRENT_DATE()";
    }

    /// <summary>
    /// Returns the current time.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CURTIME(); -&gt; '14:30:00'
    /// </code>
    /// </example>
    public static string CURTIME()
    {
        return "CURTIME()";
    }

    /// <summary>
    /// Synonym for <see cref="CURTIME"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CURRENT_TIME(); -&gt; '14:30:00'
    /// </code>
    /// </example>
    public static string CURRENT_TIME()
    {
        return "CURRENT_TIME()";
    }

    /// <summary>
    /// Returns the current date and time.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0.5 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CURRENT_TIMESTAMP(); -&gt; '2024-01-15 14:30:00'
    /// </code>
    /// </example>
    public static string CURRENT_TIMESTAMP()
    {
        return "CURRENT_TIMESTAMP()";
    }

    /// <summary>
    /// Synonym for <see cref="NOW"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0.5 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LOCALTIME(); -&gt; '2024-01-15 14:30:00'
    /// </code>
    /// </example>
    public static string LOCALTIME()
    {
        return "LOCALTIME()";
    }

    /// <summary>
    /// Synonym for <see cref="NOW"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0.5 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LOCALTIMESTAMP(); -&gt; '2024-01-15 14:30:00'
    /// </code>
    /// </example>
    public static string LOCALTIMESTAMP()
    {
        return "LOCALTIMESTAMP()";
    }

    /// <summary>
    /// Returns the time at which the function executes.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SYSDATE(); -&gt; '2024-01-15 14:30:00'
    /// </code>
    /// </example>
    public static string SYSDATE()
    {
        return "SYSDATE()";
    }

    /// <summary>
    /// Returns a Unix timestamp representing the current time.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT UNIX_TIMESTAMP(); -&gt; 1705329000
    /// </code>
    /// </example>
    public static string UNIX_TIMESTAMP()
    {
        return "UNIX_TIMESTAMP()";
    }

    /// <summary>
    /// Returns the Unix timestamp for the given <paramref name="date"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT UNIX_TIMESTAMP('2024-01-15 14:30:00'); -&gt; 1705329000
    /// </code>
    /// </example>
    public static string UNIX_TIMESTAMP(string date)
    {
        return $"UNIX_TIMESTAMP({date})";
    }

    /// <summary>
    /// Returns a date representation of the given Unix <paramref name="unixTimestamp"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT FROM_UNIXTIME(1705329000); -&gt; '2024-01-15 14:30:00'
    /// </code>
    /// </example>
    public static string FROM_UNIXTIME(string unixTimestamp)
    {
        return $"FROM_UNIXTIME({unixTimestamp})";
    }

    /// <summary>
    /// Returns a date formatted according to <paramref name="format"/> from the Unix <paramref name="unixTimestamp"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT FROM_UNIXTIME(1705329000, '%Y-%m-%d'); -&gt; '2024-01-15'
    /// </code>
    /// </example>
    public static string FROM_UNIXTIME(string unixTimestamp, string format)
    {
        return $"FROM_UNIXTIME({unixTimestamp}, {format})";
    }

    /// <summary>
    /// Returns the current UTC date.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT UTC_DATE(); -&gt; '2024-01-15'
    /// </code>
    /// </example>
    public static string UTC_DATE()
    {
        return "UTC_DATE()";
    }

    /// <summary>
    /// Returns the current UTC time.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT UTC_TIME(); -&gt; '11:00:00'
    /// </code>
    /// </example>
    public static string UTC_TIME()
    {
        return "UTC_TIME()";
    }

    /// <summary>
    /// Returns the current UTC date and time.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT UTC_TIMESTAMP(); -&gt; '2024-01-15 11:00:00'
    /// </code>
    /// </example>
    public static string UTC_TIMESTAMP()
    {
        return "UTC_TIMESTAMP()";
    }

    /// <summary>
    /// Formats <paramref name="date"/> according to the <paramref name="format"/> string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DATE_FORMAT('2024-01-15', '%W, %M %d %Y'); -&gt; 'Monday, January 15 2024'
    /// </code>
    /// </example>
    public static string DATE_FORMAT(string date, string format)
    {
        return $"DATE_FORMAT({date}, {format})";
    }

    /// <summary>
    /// Formats <paramref name="time"/> according to the <paramref name="format"/> string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TIME_FORMAT('15:30:45', '%H:%i'); -&gt; '15:30'
    /// </code>
    /// </example>
    public static string TIME_FORMAT(string time, string format)
    {
        return $"TIME_FORMAT({time}, {format})";
    }

    /// <summary>
    /// Parses <paramref name="str"/> into a date/datetime using the given <paramref name="format"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT STR_TO_DATE('15-01-2024', '%d-%m-%Y'); -&gt; '2024-01-15'
    /// </code>
    /// </example>
    public static string STR_TO_DATE(string str, string format)
    {
        return $"STR_TO_DATE({str}, {format})";
    }

    /// <summary>
    /// Returns the date for the given day number <paramref name="days"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT FROM_DAYS(738886); -&gt; '2024-01-15'
    /// </code>
    /// </example>
    public static string FROM_DAYS(string days)
    {
        return $"FROM_DAYS({days})";
    }

    /// <summary>
    /// Returns the day number of the given <paramref name="date"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TO_DAYS('2024-01-15'); -&gt; 738886
    /// </code>
    /// </example>
    public static string TO_DAYS(string date)
    {
        return $"TO_DAYS({date})";
    }

    /// <summary>
    /// Adds an <paramref name="interval"/> to <paramref name="date"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DATE_ADD('2024-01-15', INTERVAL 7 DAY); -&gt; '2024-01-22'
    /// </code>
    /// </example>
    public static string DATE_ADD(string date, string interval)
    {
        return $"DATE_ADD({date}, {interval})";
    }

    /// <summary>
    /// Synonym for <see cref="DATE_ADD"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ADDDATE('2024-01-15', INTERVAL 1 MONTH); -&gt; '2024-02-15'
    /// </code>
    /// </example>
    public static string ADDDATE(string date, string interval)
    {
        return $"ADDDATE({date}, {interval})";
    }

    /// <summary>
    /// Subtracts an <paramref name="interval"/> from <paramref name="date"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DATE_SUB('2024-01-15', INTERVAL 7 DAY); -&gt; '2024-01-08'
    /// </code>
    /// </example>
    public static string DATE_SUB(string date, string interval)
    {
        return $"DATE_SUB({date}, {interval})";
    }

    /// <summary>
    /// Synonym for <see cref="DATE_SUB"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SUBDATE('2024-01-15', INTERVAL 1 MONTH); -&gt; '2023-12-15'
    /// </code>
    /// </example>
    public static string SUBDATE(string date, string interval)
    {
        return $"SUBDATE({date}, {interval})";
    }

    /// <summary>
    /// Returns the number of days between <paramref name="date1"/> and <paramref name="date2"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DATEDIFF('2024-01-22', '2024-01-15'); -&gt; 7
    /// </code>
    /// </example>
    public static string DATEDIFF(string date1, string date2)
    {
        return $"DATEDIFF({date1}, {date2})";
    }

    /// <summary>
    /// Returns the difference between two time/datetime expressions.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TIMEDIFF('12:30:00', '10:00:00'); -&gt; '02:30:00'
    /// </code>
    /// </example>
    public static string TIMEDIFF(string expr1, string expr2)
    {
        return $"TIMEDIFF({expr1}, {expr2})";
    }

    /// <summary>
    /// Returns the difference of two date/datetime expressions in the specified <paramref name="unit"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TIMESTAMPDIFF(MONTH, '2023-01-15', '2024-01-15'); -&gt; 12
    /// </code>
    /// </example>
    public static string TIMESTAMPDIFF(string unit, string expr1, string expr2)
    {
        return $"TIMESTAMPDIFF({unit}, {expr1}, {expr2})";
    }

    /// <summary>
    /// Adds the integer <paramref name="interval"/> to <paramref name="dateTime"/> in the specified <paramref name="unit"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TIMESTAMPADD(DAY, 7, '2024-01-15'); -&gt; '2024-01-22 00:00:00'
    /// </code>
    /// </example>
    public static string TIMESTAMPADD(string unit, string interval, string dateTime)
    {
        return $"TIMESTAMPADD({unit}, {interval}, {dateTime})";
    }

    /// <summary>
    /// Extracts the date part of the expression <paramref name="dateTime"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DATE('2024-01-15 14:30:00'); -&gt; '2024-01-15'
    /// </code>
    /// </example>
    public static string DATE(string dateTime)
    {
        return $"DATE({dateTime})";
    }

    /// <summary>
    /// Extracts the time part of the expression <paramref name="dateTime"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TIME('2024-01-15 14:30:00'); -&gt; '14:30:00'
    /// </code>
    /// </example>
    public static string TIME(string dateTime)
    {
        return $"TIME({dateTime})";
    }

    /// <summary>
    /// Returns the year of <paramref name="date"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT YEAR('2024-01-15'); -&gt; 2024
    /// </code>
    /// </example>
    public static string YEAR(string date)
    {
        return $"YEAR({date})";
    }

    /// <summary>
    /// Returns the quarter of <paramref name="date"/> (1 to 4).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT QUARTER('2024-04-15'); -&gt; 2
    /// </code>
    /// </example>
    public static string QUARTER(string date)
    {
        return $"QUARTER({date})";
    }

    /// <summary>
    /// Returns the month of <paramref name="date"/> (1 to 12).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT MONTH('2024-01-15'); -&gt; 1
    /// </code>
    /// </example>
    public static string MONTH(string date)
    {
        return $"MONTH({date})";
    }

    /// <summary>
    /// Returns the week number of <paramref name="date"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT WEEK('2024-01-15'); -&gt; 3
    /// </code>
    /// </example>
    public static string WEEK(string date)
    {
        return $"WEEK({date})";
    }

    /// <summary>
    /// Returns the week number of <paramref name="date"/> using the specified <paramref name="mode"/>.
    /// </summary>
    /// <remarks>
    /// The <paramref name="mode"/> argument is available since MySQL 4.0.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT WEEK('2024-01-15', 1); -&gt; 2
    /// </code>
    /// </example>
    public static string WEEK(string date, string mode)
    {
        return $"WEEK({date}, {mode})";
    }

    /// <summary>
    /// Returns the weekday index of <paramref name="date"/> (0 = Monday, ..., 6 = Sunday).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT WEEKDAY('2024-01-15'); -&gt; 0
    /// </code>
    /// </example>
    public static string WEEKDAY(string date)
    {
        return $"WEEKDAY({date})";
    }

    /// <summary>
    /// Returns the calendar week of <paramref name="date"/> (1 to 53).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT WEEKOFYEAR('2024-01-15'); -&gt; 3
    /// </code>
    /// </example>
    public static string WEEKOFYEAR(string date)
    {
        return $"WEEKOFYEAR({date})";
    }

    /// <summary>
    /// Synonym for <see cref="DAYOFMONTH"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DAY('2024-01-15'); -&gt; 15
    /// </code>
    /// </example>
    public static string DAY(string date)
    {
        return $"DAY({date})";
    }

    /// <summary>
    /// Returns the day-of-week index of <paramref name="date"/> (1 = Sunday, ..., 7 = Saturday).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DAYOFWEEK('2024-01-15'); -&gt; 2
    /// </code>
    /// </example>
    public static string DAYOFWEEK(string date)
    {
        return $"DAYOFWEEK({date})";
    }

    /// <summary>
    /// Returns the day-of-month of <paramref name="date"/> (1 to 31).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DAYOFMONTH('2024-01-15'); -&gt; 15
    /// </code>
    /// </example>
    public static string DAYOFMONTH(string date)
    {
        return $"DAYOFMONTH({date})";
    }

    /// <summary>
    /// Returns the day-of-year of <paramref name="date"/> (1 to 366).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DAYOFYEAR('2024-01-15'); -&gt; 15
    /// </code>
    /// </example>
    public static string DAYOFYEAR(string date)
    {
        return $"DAYOFYEAR({date})";
    }

    /// <summary>
    /// Returns the name of the weekday for <paramref name="date"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DAYNAME('2024-01-15'); -&gt; 'Monday'
    /// </code>
    /// </example>
    public static string DAYNAME(string date)
    {
        return $"DAYNAME({date})";
    }

    /// <summary>
    /// Returns the name of the month for <paramref name="date"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT MONTHNAME('2024-01-15'); -&gt; 'January'
    /// </code>
    /// </example>
    public static string MONTHNAME(string date)
    {
        return $"MONTHNAME({date})";
    }

    /// <summary>
    /// Returns the year and week for <paramref name="date"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT YEARWEEK('2024-01-15'); -&gt; 202403
    /// </code>
    /// </example>
    public static string YEARWEEK(string date)
    {
        return $"YEARWEEK({date})";
    }

    /// <summary>
    /// Returns the year and week for <paramref name="date"/> using the specified <paramref name="mode"/>.
    /// </summary>
    /// <remarks>
    /// The <paramref name="mode"/> argument is available since MySQL 4.0.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT YEARWEEK('2024-01-15', 1); -&gt; 202402
    /// </code>
    /// </example>
    public static string YEARWEEK(string date, string mode)
    {
        return $"YEARWEEK({date}, {mode})";
    }

    /// <summary>
    /// Returns the hour of <paramref name="time"/> (0 to 23).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT HOUR('14:30:00'); -&gt; 14
    /// </code>
    /// </example>
    public static string HOUR(string time)
    {
        return $"HOUR({time})";
    }

    /// <summary>
    /// Returns the minute of <paramref name="time"/> (0 to 59).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT MINUTE('14:30:00'); -&gt; 30
    /// </code>
    /// </example>
    public static string MINUTE(string time)
    {
        return $"MINUTE({time})";
    }

    /// <summary>
    /// Returns the seconds of <paramref name="time"/> (0 to 59).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SECOND('14:30:45'); -&gt; 45
    /// </code>
    /// </example>
    public static string SECOND(string time)
    {
        return $"SECOND({time})";
    }

    /// <summary>
    /// Returns the microseconds of <paramref name="expr"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT MICROSECOND('14:30:00.123456'); -&gt; 123456
    /// </code>
    /// </example>
    public static string MICROSECOND(string expr)
    {
        return $"MICROSECOND({expr})";
    }

    /// <summary>
    /// Extracts the specified <paramref name="unit"/> from the date <paramref name="date"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT EXTRACT(YEAR FROM '2024-01-15'); -&gt; 2024
    /// </code>
    /// </example>
    public static string EXTRACT(string unit, string date)
    {
        return $"EXTRACT({unit} FROM {date})";
    }

    /// <summary>
    /// Returns the last day of the month for the given <paramref name="date"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LAST_DAY('2024-02-15'); -&gt; '2024-02-29'
    /// </code>
    /// </example>
    public static string LAST_DAY(string date)
    {
        return $"LAST_DAY({date})";
    }

    /// <summary>
    /// Returns a date from a year and day-of-year value.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT MAKEDATE(2024, 15); -&gt; '2024-01-15'
    /// </code>
    /// </example>
    public static string MAKEDATE(string year, string dayOfYear)
    {
        return $"MAKEDATE({year}, {dayOfYear})";
    }

    /// <summary>
    /// Returns a time value from the given <paramref name="hour"/>, <paramref name="minute"/>, and <paramref name="second"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT MAKETIME(14, 30, 45); -&gt; '14:30:45'
    /// </code>
    /// </example>
    public static string MAKETIME(string hour, string minute, string second)
    {
        return $"MAKETIME({hour}, {minute}, {second})";
    }

    /// <summary>
    /// Adds <paramref name="months"/> to a period (in the format YYMM or YYYYMM).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT PERIOD_ADD(202401, 3); -&gt; 202404
    /// </code>
    /// </example>
    public static string PERIOD_ADD(string period, string months)
    {
        return $"PERIOD_ADD({period}, {months})";
    }

    /// <summary>
    /// Returns the number of months between periods <paramref name="period1"/> and <paramref name="period2"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT PERIOD_DIFF(202404, 202401); -&gt; 3
    /// </code>
    /// </example>
    public static string PERIOD_DIFF(string period1, string period2)
    {
        return $"PERIOD_DIFF({period1}, {period2})";
    }

    /// <summary>
    /// Converts <paramref name="seconds"/> to a TIME value.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SEC_TO_TIME(3661); -&gt; '01:01:01'
    /// </code>
    /// </example>
    public static string SEC_TO_TIME(string seconds)
    {
        return $"SEC_TO_TIME({seconds})";
    }

    /// <summary>
    /// Returns the seconds argument converted to an integer representing the seconds of <paramref name="time"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TIME_TO_SEC('01:01:01'); -&gt; 3661
    /// </code>
    /// </example>
    public static string TIME_TO_SEC(string time)
    {
        return $"TIME_TO_SEC({time})";
    }

    /// <summary>
    /// Converts a datetime value <paramref name="dt"/> from <paramref name="fromTz"/> to <paramref name="toTz"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.3 and MariaDB 5.1. Requires the time zone tables to be populated.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CONVERT_TZ('2024-01-15 14:30:00', '+00:00', '+05:30'); -&gt; '2024-01-15 20:00:00'
    /// </code>
    /// </example>
    public static string CONVERT_TZ(string dt, string fromTz, string toTz)
    {
        return $"CONVERT_TZ({dt}, {fromTz}, {toTz})";
    }

    /// <summary>
    /// Returns a format string suitable for use with DATE_FORMAT or STR_TO_DATE.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT GET_FORMAT(DATE, 'EUR'); -&gt; '%d.%m.%Y'
    /// </code>
    /// </example>
    public static string GET_FORMAT(string type, string location)
    {
        return $"GET_FORMAT({type}, {location})";
    }

    /// <summary>
    /// Adds <paramref name="time"/> to <paramref name="expr1"/> and returns the result.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ADDTIME('2024-01-15 14:30:00', '01:00:00'); -&gt; '2024-01-15 15:30:00'
    /// </code>
    /// </example>
    public static string ADDTIME(string expr1, string time)
    {
        return $"ADDTIME({expr1}, {time})";
    }

    /// <summary>
    /// Subtracts <paramref name="time"/> from <paramref name="expr1"/> and returns the result.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SUBTIME('2024-01-15 14:30:00', '01:00:00'); -&gt; '2024-01-15 13:30:00'
    /// </code>
    /// </example>
    public static string SUBTIME(string expr1, string time)
    {
        return $"SUBTIME({expr1}, {time})";
    }

    /// <summary>
    /// With a single argument, returns the date or datetime expression <paramref name="expr"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TIMESTAMP('2024-01-15'); -&gt; '2024-01-15 00:00:00'
    /// </code>
    /// </example>
    public static string TIMESTAMP(string expr)
    {
        return $"TIMESTAMP({expr})";
    }

    /// <summary>
    /// With two arguments, adds the time expression <paramref name="time"/> to the date or datetime expression <paramref name="expr"/> and returns the resulting datetime value.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TIMESTAMP('2024-01-15', '14:30:00'); -&gt; '2024-01-15 14:30:00'
    /// </code>
    /// </example>
    public static string TIMESTAMP(string expr, string time)
    {
        return $"TIMESTAMP({expr}, {time})";
    }

    /// <summary>
    /// Returns the date or datetime argument <paramref name="date"/> converted to seconds since Year 0.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.0.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TO_SECONDS('2024-01-15 14:30:00'); -&gt; 63835072200
    /// </code>
    /// </example>
    public static string TO_SECONDS(string date)
    {
        return $"TO_SECONDS({date})";
    }
}