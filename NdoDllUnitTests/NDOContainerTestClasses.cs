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

    public interface ICarSelector
    {
        ICar Car { get; }
        string Name { get; }
    }

    public class CarSelector : ICarSelector
    {
		private readonly string name;
		private readonly ICar car;

		public CarSelector(string name, ICar car)
        {
			this.name = name;
			this.car = car;
		}

		public ICar Car => car;

		public string Name => name;
	}

    public class Driver
    {
        private ICar _car = null;

        public Driver(Func<ICar> carFactory)
        {
            _car = carFactory();
        }


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
