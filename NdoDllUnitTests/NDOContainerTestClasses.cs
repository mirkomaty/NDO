using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// This is from https://www.tutorialsteacher.com/ioc/register-and-resolve-in-unity-container
namespace NdoDllUnitTests
{


    public interface ICar
    {
        int Run();
    }

    public class BMW : ICar
    {
        private int _miles = 0;

        public int Run()
        {
            return ++_miles;
        }
    }

    public class Ford : ICar
    {
        private int _miles = 0;

        public int Run()
        {
            return ++_miles;
        }
    }

    public class Audi : ICar
    {
        private int _miles = 0;

        public int Run()
        {
            return ++_miles;
        }

    }
    public class Driver
    {
        private ICar _car = null;

        public Driver( ICar car )
        {
            _car = car;
        }

        public string RunCar()
        {
            return $"Running {_car.GetType().Name} - {_car.Run()} mile";
        }
    }

}
