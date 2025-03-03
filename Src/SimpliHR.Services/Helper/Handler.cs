using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Services
{
    public class SqlTimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
    {
        public override void SetValue(IDbDataParameter parameter, TimeOnly time)
        {
            parameter.Value = time.ToString();
        }

        public override TimeOnly Parse(object value)
        {
            return TimeOnly.FromTimeSpan((TimeSpan)value);
        }
    }

    public class DapperSqlDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly date)
        => parameter.Value = date.ToDateTime(new TimeOnly(0, 0));
    
    public override DateOnly Parse(object value)
        => DateOnly.FromDateTime((DateTime)value);
}



    //public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly> // Dapper handler for DateOnly
    //{
    //    public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);

    //    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    //    {
    //        parameter.DbType = DbType.Date;
    //        parameter.Value = value;
    //    }
    //}

    //public class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly> // Dapper handler for TimeOnly
    //{
    //    public override TimeOnly Parse(object value)
    //    {
    //        if (value.GetType() == typeof(DateTime))
    //        {
    //            return TimeOnly.FromDateTime((DateTime)value);
    //        }
    //        else if (value.GetType() == typeof(TimeSpan))
    //        {
    //            return TimeOnly.FromTimeSpan((TimeSpan)value);
    //        }
    //        return default;
    //    }

    //    public override void SetValue(IDbDataParameter parameter, TimeOnly value)
    //    {
    //        parameter.DbType = DbType.Time;
    //        parameter.Value = value;
    //    }
    //}
}
