using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DroneTrack.Source.Models;
using Microsoft.Data.Sqlite;

namespace DroneTrack.Source.Data
{
    public class DatabaseService
    {
        DroneDatabaseContext database = new DroneDatabaseContext();

        public DatabaseService()
        {
            database.Database.EnsureCreated();
        }
    }
}
