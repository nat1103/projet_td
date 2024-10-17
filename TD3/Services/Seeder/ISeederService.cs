using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD3.Services.Seeder
{
    internal interface ISeederService
    {
        int Order { get; }
        void SeedData();
    }
}
