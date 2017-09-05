using LibDnaSerial;
using ModMonitor.Utils;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModMonitor.Models
{
    class StatisticsDatabase : DbContext
    {
        public static StatisticsDatabase Open()
        {
            string dsn = string.Format("Data Source={0};Version=3;", Path.Combine((Application.Current as App).DataPath, "Statistics.db"));
            return new StatisticsDatabase(dsn);
        }

        public StatisticsDatabase(string connectionString) : base(connectionString) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<StatisticsDatabase>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }

        public string GetLatestRecordNote()
        {
            var record = Statistics.OrderBy(r => r.Timestamp).FirstOrDefault();
            if (record != null)
            {
                return record.Note;
            }
            return "";
        }

        public DbSet<Statistics> Statistics { get; set; }
    }

    class Statistics
    {
        [Key]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string Note { get; set; }

        public int Puffs { get; set; }

        public int TemperatureProtectedPuffs { get; set; }

        public float MeanEnergy { get; set; }

        public float MeanPower { get; set; }

        public DbTemperature MeanTemperature { get; set; }
        
        public DbTemperature MeanTemperaturePeak { get; set; }

        public float MeanTime { get; set; }

        public float StdDevEnergy { get; set; }

        public float StdDevPower { get; set; }

        public DbTemperature StdDevTemperature { get; set; }

        public DbTemperature StdDevTemperaturePeak { get; set; }

        public float StdDevTime { get; set; }

        public float TotalEnergy { get; set; }

        public float TotalTime { get; set; }
    }

    [ComplexType]
    [CsvHeader("Value", "Unit")]
    public class DbTemperature : CsvValue
    {
        public float Value { get; set; }

        public TemperatureUnit Unit { get; set; }

        public static DbTemperature FromTemperature(Temperature t)
        {
            if (t == null) return new DbTemperature { Unit = TemperatureUnit.F, Value = 0f };
            return new DbTemperature { Unit = t.Unit, Value = t.Value };
        }

        public Temperature ToTemperature()
        {
            return new Temperature { Unit = Unit, Value = Value };
        }

        public override string ToString()
        {
            return ToTemperature().ToString();
        }

        public string[] GetCsvValues()
        {
            return new string[] { Value.ToString(), Unit.ToString() };
        }
    }
}
