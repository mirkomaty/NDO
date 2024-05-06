using NDO.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NdoDllUnitTests
{
	[TestFixture]
	public class NDOContainerTests
	{
		[Test]
		public void SimpleRegistration()
		{
			INDOContainer container = new NDOContainer();

			container.RegisterType<ICar, BMW>();
			container.RegisterType<Driver>();

			//Resolves dependencies and returns the Driver object 
			Driver drv = container.Resolve<Driver>();
			Assert.That( "Running BMW - 1 mile", Is.EqualTo( drv.RunCar() ) );
		}

		[Test]
		public void SimpleRegistrationWithOneType()
		{
			INDOContainer container = new NDOContainer();

			container.RegisterType<BMW>();

			var car = container.Resolve<BMW>();
			Assert.That( typeof( BMW ), Is.EqualTo( car.GetType() ) );
		}

		[Test]
		public void SimpleRegistrationWithOverride()
		{
			INDOContainer container = new NDOContainer();

			container.RegisterType<Driver>();

			//Resolves dependencies and returns the Driver object 
			Driver drv = container.Resolve<Driver>(null, new ParameterOverride("car", new BMW()));
			Assert.That( "Running BMW - 1 mile", Is.EqualTo( drv.RunCar() ) );
		}

		[Test]
		public void RegistrationWithAnonymousOverride()
		{
			INDOContainer container = new NDOContainer();

			container.RegisterType<Driver>();

			//Resolves dependencies and returns the Driver object 
			Driver drv = container.Resolve<Driver>(null, new ParameterOverride(new BMW()));
			Assert.That( "Running BMW - 1 mile", Is.EqualTo( drv.RunCar() ) );
		}

		[Test]
		public void DoubleResolve()
		{
			INDOContainer container = new NDOContainer();

			container.RegisterType<ICar, BMW>(new ContainerControlledLifetimeManager());

			var car1 = container.Resolve<ICar>();
			var car2 = container.Resolve<ICar>();

			Assert.That( Object.ReferenceEquals( car1, car2 ) );
		}

		[Test]
		public void DoubleResolveWithName()
		{
			INDOContainer container = new NDOContainer();

			container.RegisterType<ICar, BMW>(new ContainerControlledLifetimeManager());

			var car1 = container.Resolve<ICar>("car1");
			var car2 = container.Resolve<ICar>("car1");
			var car3 = container.Resolve<ICar>("car2");

			Assert.That( Object.ReferenceEquals( car1, car2 ) );
			Assert.That( !Object.ReferenceEquals( car1, car3 ) );
		}

		[Test]
		public void DoubleRegisterTypeWithName()
		{
			INDOContainer container = new NDOContainer();

			container.RegisterType<ICar, BMW>( new ContainerControlledLifetimeManager() );
			container.RegisterType<ICar, Audi>( new ContainerControlledLifetimeManager() );

			var car1 = container.Resolve<ICar>( "bmw" );
			var car2 = container.Resolve<ICar>( "bmw" );
			var car3 = container.Resolve<ICar>( "audi" );

			Assert.That( Object.ReferenceEquals( car1, car2 ) );
			Assert.That( !Object.ReferenceEquals( car1, car3 ) );
		}

		[Test]
		public void DoubleRegisterTypeWithNameTransient()
		{
			INDOContainer container = new NDOContainer();

			container.RegisterType<ICar, BMW>();
			container.RegisterType<ICar, Audi>();

			var car1 = container.Resolve<ICar>( "bmw" );
			var car2 = container.Resolve<ICar>( "bmw" );
			var car3 = container.Resolve<ICar>( "audi" );

			Assert.That( !Object.ReferenceEquals( car1, car2 ) );
			Assert.That( !Object.ReferenceEquals( car1, car3 ) );
		}

		[Test]
		public void ResolveOrRegisterTypeWithOverride()
		{
			INDOContainer container = new NDOContainer();

			var drv1 = container.ResolveOrRegisterType<Driver>(new ContainerControlledLifetimeManager(), "bmw", new ParameterOverride("car", new BMW()));
			var drv2 = container.ResolveOrRegisterType<Driver>(new ContainerControlledLifetimeManager(), "audi", new ParameterOverride("car", new Audi()));
			Assert.That( "Running BMW - 1 mile", Is.EqualTo( drv1.RunCar() ) );
			Assert.That( "Running Audi - 1 mile", Is.EqualTo( drv2.RunCar() ) );
			drv1 = container.ResolveOrRegisterType<Driver>( new ContainerControlledLifetimeManager(), "bmw", new ParameterOverride( "car", new BMW() ) );
			drv2 = container.ResolveOrRegisterType<Driver>( new ContainerControlledLifetimeManager(), "audi", new ParameterOverride( "car", new Audi() ) );
			Assert.That( "Running BMW - 2 mile", Is.EqualTo( drv1.RunCar() ) );
			Assert.That( "Running Audi - 2 mile", Is.EqualTo( drv2.RunCar() ) );
		}

		[Test]
		public void RegisterInstance()
		{
			INDOContainer container = new NDOContainer();

			var car1 = new BMW();
			container.RegisterInstance<ICar>( car1 );

			var car2 = container.Resolve<ICar>();
			Assert.That( Object.ReferenceEquals( car1, car2 ) );
		}

		[Test]
		public void RegisterInstanceWithName()
		{
			INDOContainer container = new NDOContainer();

			var bmw1 = new BMW();
			container.RegisterInstance<ICar>( bmw1, "bmw" );
			var audi1 = new Audi();
			container.RegisterInstance<ICar>( audi1, "audi" );

			var bmw2 = container.Resolve<ICar>("bmw");
			Assert.That( Object.ReferenceEquals( bmw1, bmw2 ) );
			var audi2 = container.Resolve<ICar>("audi");
			Assert.That( Object.ReferenceEquals( audi1, audi2 ) );
		}

		[Test]
		public void ResolveAndRegisterInstance()
		{
			INDOContainer container = new NDOContainer();
			var car1 = container.ResolveOrRegisterInstance<ICar>("bmw", _=>new BMW());
			var car2 = container.ResolveOrRegisterInstance<ICar>("bmw", _=>new BMW());
			Assert.That( Object.ReferenceEquals( car1, car2 ) );

			var car3 = container.ResolveOrRegisterInstance<ICar>("ford", _=>new Ford());
			Assert.That( !Object.ReferenceEquals( car1, car3 ) );

			var car4 = container.ResolveOrRegisterInstance<ICar>("bmw2", _=>new BMW());
			Assert.That( !Object.ReferenceEquals( car1, car4 ) );
		}

		[Test]
		public void ResolveAndRegisterInstanceWithFactory()
		{
			INDOContainer container = new NDOContainer();
			container.ResolveOrRegisterInstance( "bmw", _ => new BMW() );
			var bmw = container.Resolve( typeof( BMW ), "bmw" );
			Assert.That( bmw != null );
			Assert.That( typeof( BMW ) == bmw.GetType() );
			var bmw2 = container.Resolve( typeof( BMW ), "bmw" );
			Assert.That( Object.ReferenceEquals( bmw, bmw2 ) );
		}


		[Test]
		public void ResolveWithChildContainer()
		{
			INDOContainer container1 = new NDOContainer();

			container1.RegisterType<ICar, BMW>( new ContainerControlledLifetimeManager() );

			var car1 = container1.Resolve<ICar>();


			var container2 = container1.CreateChildContainer();
			var car2 = container2.Resolve<ICar>();

			Assert.That( Object.ReferenceEquals( car1, car2 ) );

			container2.RegisterType<ICar, BMW>( new ContainerControlledLifetimeManager() );
			car2 = container2.Resolve<ICar>();

			Assert.That( !Object.ReferenceEquals( car1, car2 ) );

			container2.Dispose();

			car2 = container1.Resolve<ICar>();
			Assert.That( Object.ReferenceEquals( car1, car2 ) );
		}

		[Test]
		public void ResolveWithChildContainer2()
		{
			INDOContainer container1 = new NDOContainer();

			container1.RegisterType<ICar, BMW>( new ContainerControlledLifetimeManager() );

			var car1 = container1.Resolve<ICar>();


			var container2 = container1.CreateChildContainer();
			var car2 = container2.Resolve<ICar>();

			Assert.That( Object.ReferenceEquals( car1, car2 ) );

			container2.RegisterType<ICar, BMW>(/* Transient */);
			car2 = container2.Resolve<ICar>();

			Assert.That( !Object.ReferenceEquals( car1, car2 ) );

			container2.Dispose();

			car2 = container1.Resolve<ICar>();
			Assert.That( Object.ReferenceEquals( car1, car2 ) );
		}

		[Test]
		public void ResolveInstanceWithChildContainer()
		{
			INDOContainer container1 = new NDOContainer();

			container1.RegisterInstance<ICar>( new BMW() );

			var car1 = container1.Resolve<ICar>();


			var container2 = container1.CreateChildContainer();
			var car2 = container2.Resolve<ICar>();

			Assert.That( Object.ReferenceEquals( car1, car2 ) );

			container2.RegisterInstance<ICar>( new BMW() );
			car2 = container2.Resolve<ICar>();

			Assert.That( !Object.ReferenceEquals( car1, car2 ) );

			container2.Dispose();

			car2 = container1.Resolve<ICar>();
			Assert.That( Object.ReferenceEquals( car1, car2 ) );
		}

		[Test]
		public void ResolveWithParameterInParentContainer()
		{
			var container1 = new NDOContainer();
			container1.RegisterType<Driver>();
			var container2 = container1.CreateChildContainer();
			container2.RegisterInstance<ICar>( new BMW() );
			var drv = container2.Resolve<Driver>();
			Assert.That( drv != null );
			Assert.That( "Running BMW - 1 mile" == drv.RunCar() );

			container1 = new NDOContainer();
			container1.RegisterInstance<ICar>( new BMW() );
			container2 = container1.CreateChildContainer();
			container2.RegisterType<Driver>();
			drv = container2.Resolve<Driver>();
			Assert.That( drv != null );
			Assert.That( "Running BMW - 1 mile" == drv.RunCar() );
		}

		[Test]
		public void RegisterWithTransientLifetime()
		{
			var container = new NDOContainer();
			container.RegisterType<ICar, BMW>( new TransientLifetimeManager() );

			var driver1 = container.Resolve<Driver>();

			Assert.That( "Running BMW - 1 mile" == driver1.RunCar() );

			var driver2 = container.Resolve<Driver>();

			Assert.That( "Running BMW - 1 mile" == driver2.RunCar() );
		}

		[Test]
		public void RegisterWithContainerLifetime()
		{
			var container = new NDOContainer();
			container.RegisterType<ICar, BMW>( new ContainerControlledLifetimeManager() );

			var driver1 = container.Resolve<Driver>();
			Assert.That( "Running BMW - 1 mile" == driver1.RunCar() );

			var driver2 = container.Resolve<Driver>();

			Assert.That( "Running BMW - 2 mile" == driver2.RunCar() );
		}

		[Test]
		public void TestNullInstance()
		{
			var container = new NDOContainer();
			container.RegisterInstance<ICar>( null );
			Assert.That( container.Resolve<ICar>() == null );
		}

		[Test]
		public void IsRegisteredWorks()
		{
			var container = new NDOContainer();
			Assert.That( !container.IsRegistered<ICar>() );
			container.RegisterInstance<ICar>( null );
			Assert.That( container.IsRegistered<ICar>() );
		}


		[Test]
		public void OverwriteTypeRegWithInstanceReg()
		{
			var container = new NDOContainer();
			container.RegisterType<ICar, BMW>();
			container.RegisterInstance<ICar>( new Ford() );
			Assert.That( container.Resolve<ICar>() is Ford );
		}

		[Test]
		public void OverwriteInstanceRegWithTypeReg()
		{
			var container = new NDOContainer();
			container.RegisterInstance<ICar>( new Ford() );
			container.RegisterType<ICar, BMW>();
			Assert.That( container.Resolve<ICar>() is BMW );
		}

		[Test]
		public void ResolveWithoutRegistration()
		{
			var container = new NDOContainer();
			Assert.That( container.Resolve<BMW>() is BMW );
		}

		[Test]
		public void ResolveInterfaceWithoutRegistrationIsNull()
		{
			var container = new NDOContainer();
			Assert.That( container.Resolve<ICar>() == null );
		}

		[Test]
		public void ResolveTypeWithInstanceDependency()
		{
			var container = new NDOContainer();
			container.RegisterInstance<ICar>( new BMW() );
			var drv = container.Resolve<Driver>();
			Assert.That( drv != null );
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
			Assert.That( 1 == count );
		}
	}
}
