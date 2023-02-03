using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NDO.Application;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NdoDllUnitTests
{
	[TestFixture]
	public class NDOContainerTests
	{
		IServiceProvider ndoServiceProvider;
		IHostBuilder builder;

		[SetUp]
		public void Setup()
		{
			this.builder = Host.CreateDefaultBuilder();
			//builder.Environment.EnvironmentName = "Test2";
		}


		void Build(Action<IServiceCollection> configure)
		{
			this.builder.ConfigureServices( services => 
			{
				services.AddNdo( null, null );
				configure( services );
			} );

			var host = this.builder.Build();
			this.ndoServiceProvider = host.Services;
			host.Services.UseNdo();
		}

		[Test]
		public void SimpleRegistration()
		{
			Build( ( serviceCollection ) =>
			{
				serviceCollection.AddSingleton<ICar, BMW>();
				serviceCollection.AddSingleton<Driver>();
			} );

			//Resolves dependencies and returns the Driver object 
			Driver drv = this.ndoServiceProvider.GetService<Driver>();
			Assert.AreEqual( "Running BMW - 1 mile", drv.RunCar() );
		}

		[Test]
		public void SimpleRegistrationWithOneType()
		{
			Build( ( serviceCollection ) =>
			{
				serviceCollection.AddSingleton<BMW>();
			} );

			var car = this.ndoServiceProvider.GetService<BMW>();
			Assert.AreEqual( typeof( BMW ), car.GetType() );
		}

		[Test]
		public void SimpleRegistrationWithResolveParameter()
		{
			Build( ( serviceCollection ) =>
			{
				serviceCollection.AddSingleton<Driver>();
				// This code defines BMW as Parameter
				serviceCollection.AddTransient( ( provider ) =>
				{
					return new Func<ICar>(
						() => new BMW()
					);
				} );
			} );

			//Resolves dependencies and returns the Driver object 
			Driver drv = this.ndoServiceProvider.GetService<Driver>();
			Assert.AreEqual( "Running BMW - 1 mile", drv.RunCar() );
		}


		[Test]
		public void DoubleResolve()
		{
			Build( ( serviceCollection ) =>
			{
				serviceCollection.AddSingleton<ICar, BMW>();
			} );
			var car1 = this.ndoServiceProvider.GetService<ICar>();
			var car2 = this.ndoServiceProvider.GetService<ICar>();

			Assert.AreSame( car1, car2 );
		}


		[Test]
		public void DoubleRegisterType()
		{
			// This replaces resolves with names introduced by the old container solution.
			// ICar could have a property with which the right service can be selected 
			Build( ( serviceCollection ) =>
			{
				serviceCollection.AddSingleton<ICar, BMW>();
				serviceCollection.AddSingleton<ICar, Audi>();
			} );

			var cars = this.ndoServiceProvider.GetServices<ICar>();

			Assert.AreEqual( 2, cars.Count() );
			Assert.AreNotSame( cars.First(), cars.Last() );
		}

		[Test]
		public void DoubleRegisterTypeWithNameTransient()
		{
			// This replaces resolves with names introduced by the old container solution.
			// ICar could have a property with which the right service can be selected 
			Build( ( serviceCollection ) =>
			{

				serviceCollection.AddTransient<ICar, BMW>();
				serviceCollection.AddTransient<ICar, Audi>();
			} );

			var cars = this.ndoServiceProvider.GetServices<ICar>();

			Assert.AreEqual( 2, cars.Count() );
			Assert.AreSame( cars.First(), cars.First() );
			Assert.AreNotSame( cars.First(), cars.Last() );
		}

		[Test]
		public void RegisterInstanceWithName()
		{
			ICarSelector carSelector = null;
			Build( ( serviceCollection ) =>
			{
				serviceCollection.AddTransient<Driver>();  // Singleton would always return the same driver
				serviceCollection.AddSingleton<ICarSelector>( new CarSelector( "bmw", new BMW() ) );
				serviceCollection.AddSingleton<ICarSelector>( new CarSelector( "audi", new Audi() ) );

				serviceCollection.AddTransient( ( provider ) =>
				{
					return new Func<ICar>(
						() => carSelector?.Car
					);
				} );
			} );

			carSelector = ndoServiceProvider.GetServices<ICarSelector>().FirstOrDefault(cs => cs.Name == "bmw");
			Assert.IsNotNull( carSelector );

			var drv1 = ndoServiceProvider.GetService<Driver>();
			Assert.AreEqual( "Running BMW - 1 mile", drv1.RunCar() );

			carSelector = ndoServiceProvider.GetServices<ICarSelector>().FirstOrDefault( cs => cs.Name == "audi" );

			var drv2 = ndoServiceProvider.GetService<Driver>();
			Assert.AreEqual( "Running Audi - 1 mile", drv2.RunCar() );
		}

		[Test]
		public void RegisterInstance()
		{
			var car1 = new BMW();
			Build( ( serviceCollection ) =>
			{
				serviceCollection.AddSingleton<ICar>( car1 );
			} );

			var car2 = this.ndoServiceProvider.GetService<ICar>();
			Assert.AreSame( car1, car2 );
		}

#if nix

		[Test]
		public void ResolveWithScopedContainer()
		{
			INDOContainer container1 = new NDOContainer();

			container1.RegisterType<ICar, BMW>();

			var car1 = container1.Resolve<ICar>();


			var container2 = container1.CreateChildContainer();
			var car2 = container2.Resolve<ICar>();

			Assert.AreSame( car1, car2 );

			container2.RegisterType<ICar, BMW>();
			car2 = container2.Resolve<ICar>();

			Assert.AreNotSame( car1, car2 );

			container2.Dispose();

			car2 = container1.Resolve<ICar>();
			Assert.AreSame( car1, car2 );
		}


		[Test]
		public void ResolveWithChildContainer2()
		{
			INDOContainer container1 = new NDOContainer();

			container1.RegisterType<ICar, BMW>();

			var car1 = container1.Resolve<ICar>();


			var container2 = container1.CreateChildContainer();
			var car2 = container2.Resolve<ICar>();

			Assert.AreSame( car1, car2 );

			container2.RegisterType<ICar, BMW>(/* Transient */);
			car2 = container2.Resolve<ICar>();

			Assert.AreNotSame( car1, car2 );

			container2.Dispose();

			car2 = container1.Resolve<ICar>();
			Assert.AreSame( car1, car2 );
		}

		[Test]
		public void ResolveInstanceWithChildContainer()
		{
			INDOContainer container1 = new NDOContainer();

			container1.RegisterInstance<ICar>( new BMW() );

			var car1 = container1.Resolve<ICar>();


			var container2 = container1.CreateChildContainer();
			var car2 = container2.Resolve<ICar>();

			Assert.AreSame( car1, car2 );

			container2.RegisterInstance<ICar>( new BMW() );
			car2 = container2.Resolve<ICar>();

			Assert.AreNotSame( car1, car2 );

			container2.Dispose();

			car2 = container1.Resolve<ICar>();
			Assert.AreSame( car1, car2 );
		}

		[Test]
		public void ResolveWithParameterInParentContainer()
		{
			var container1 = new NDOContainer();
			container1.RegisterType<Driver>();
			var container2 = container1.CreateChildContainer();
			container2.RegisterInstance<ICar>( new BMW() );
			var drv = container2.Resolve<Driver>();
			Assert.NotNull( drv );
			Assert.AreEqual( "Running BMW - 1 mile", drv.RunCar() );

			container1 = new NDOContainer();
			container1.RegisterInstance<ICar>( new BMW() );
			container2 = container1.CreateChildContainer();
			container2.RegisterType<Driver>();
			drv = container2.Resolve<Driver>();
			Assert.NotNull( drv );
			Assert.AreEqual( "Running BMW - 1 mile", drv.RunCar() );
		}

		[Test]
		public void RegisterWithTransientLifetime()
		{
			var container = new NDOContainer();
			serviceCollection.AddSingleton<ICar, BMW>( new TransientLifetimeManager() );

			var driver1 = this.ndoServiceProvider.GetService<Driver>();

			Assert.AreEqual( "Running BMW - 1 mile", driver1.RunCar() );

			var driver2 = this.ndoServiceProvider.GetService<Driver>();

			Assert.AreEqual( "Running BMW - 1 mile", driver2.RunCar() );
		}

		[Test]
		public void RegisterWithContainerLifetime()
		{
			var container = new NDOContainer();
			serviceCollection.AddSingleton<ICar, BMW>();

			var driver1 = this.ndoServiceProvider.GetService<Driver>();
			Assert.AreEqual( "Running BMW - 1 mile", driver1.RunCar() );

			var driver2 = this.ndoServiceProvider.GetService<Driver>();

			Assert.AreEqual( "Running BMW - 2 mile", driver2.RunCar() );
		}

		[Test]
		public void TestNullInstance()
		{
			var container = new NDOContainer();
			serviceCollection.AddSingleton<ICar>( null );
			Assert.IsNull( this.ndoServiceProvider.GetService<ICar>() );
		}

		[Test]
		public void IsRegisteredWorks()
		{
			var container = new NDOContainer();
			Assert.AreEqual( false, container.IsRegistered<ICar>() );
			serviceCollection.AddSingleton<ICar>( null );
			Assert.AreEqual( true, container.IsRegistered<ICar>() );
		}


		[Test]
		public void OverwriteTypeRegWithInstanceReg()
		{
			var container = new NDOContainer();
			serviceCollection.AddSingleton<ICar, BMW>();
			serviceCollection.AddSingleton<ICar>( new Ford() );
			Assert.IsTrue( this.ndoServiceProvider.GetService<ICar>() is Ford );
		}

		[Test]
		public void OverwriteInstanceRegWithTypeReg()
		{
			var container = new NDOContainer();
			serviceCollection.AddSingleton<ICar>( new Ford() );
			serviceCollection.AddSingleton<ICar, BMW>();
			Assert.IsTrue( this.ndoServiceProvider.GetService<ICar>() is BMW );
		}

		[Test]
		public void ResolveWithoutRegistration()
		{
			var container = new NDOContainer();
			Assert.IsTrue( this.ndoServiceProvider.GetService<BMW>() is BMW );
		}

		[Test]
		public void ResolveInterfaceWithoutRegistrationIsNull()
		{
			var container = new NDOContainer();
			Assert.IsNull( this.ndoServiceProvider.GetService<ICar>() );
		}

		[Test]
		public void ResolveTypeWithInstanceDependency()
		{
			var container = new NDOContainer();
			serviceCollection.AddSingleton<ICar>( new BMW() );
			var drv = this.ndoServiceProvider.GetService<Driver>();
			Assert.NotNull( drv );
		}

		class TestDisposable : IDisposable
		{
			public void Dispose()
			{
			}
		}

		[Test]
		public void DisposeTest()
		{
			ConcurrentDictionary<string, object> dict = new ConcurrentDictionary<string, object>();
			var o1 = new object();
			var o2 = new TestDisposable();
			dict.TryAdd( "o1", o1 );
			dict.TryAdd( "o2", o2 );
			int count = 0;
			foreach (IDisposable d in dict.Values.Where( o => o is IDisposable )) 
			{
				count++;
			}
			Assert.AreEqual( 1, count );
		}
#endif
	}
}
