using Interface;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface IMoodRepository
    {
        List<Mood> GetAll(MoodFilter filter);
        void NewSeedData();
        //int AddMood();
    }
}
