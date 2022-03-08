using NUnit.Framework;
using System;
using NDO.Configuration;

namespace NdoDllUnitTests
{
	[TestFixture]
	public class NDOContainerTests
	{
		[Test]
		public void SimpleRegistration()
		{
			INDOContainer container = new NDOContainer();

			container.Register<ICar, BMW>();
			container.Register<Driver>();

			//Resolves dependencies and returns the Driver object 
			Driver drv = container.GetInstance<Driver>();
			Assert.AreEqual( "Running BMW - 1 mile", drv.RunCar() );
		}

		[Test]
		public void SimpleRegistrationWithOneType()
		{
			INDOContainer container = new NDOContainer();

			container.Register<BMW>();

			var car = container.GetInstance<BMW>();
			Assert.AreEqual( typeof( BMW ), car.GetType() );
		}

		[Test]
		public void GetUnregistered()
		{
			INDOContainer container = new NDOContainer();
			var car = container.TryGetInstance<ICar>();
			Assert.IsNull( car );
		}

		[Test]
		public void SimpleRegistrationWithNonRegisteredParameter()
		{
			INDOContainer container = new NDOContainer();

			container.Register<int, ICar>( ( factory, i ) => new Hyunday( i ) );
			container.Register<Driver>();

			var instanceFactory = container.GetInstance<Func<int, ICar>>();
			var car = instanceFactory( 42 );

			//Resolves dependencies and returns the Driver object 
			Driver drv = new Driver( car );
			Console.WriteLine( drv.RunCar() );
		}

		[Test]
		public void DoubleResolve()
		{
			INDOContainer container = new NDOContainer();

			container.Register<ICar, BMW>( new PerScopeLifetime() );
			
			var car1 = container.GetInstance<ICar>();
			var car2 = container.GetInstance<ICar>();
			Assert.AreSame( car1, car2 );
			
		}

		[Test]
		public void DoubleResolveWithName()
		{
			INDOContainer container = new NDOContainer();

			container.Register<ICar, BMW>( "car1", new PerContainerLifetime() );
			container.Register<ICar, BMW>( "car2", new PerContainerLifetime() );

			var car1 = container.GetInstance<ICar>( "car1" );
			var car2 = container.GetInstance<ICar>( "car1" );
			var car3 = container.GetInstance<ICar>( "car2" );

			Assert.AreSame( car1, car2 );
			Assert.AreNotSame( car1, car3 );
		}

		[Test]
		public void DoubleRegisterTypeWithName()
		{
			INDOContainer container = new NDOContainer();

			container.Register<ICar, BMW>( "bmw" );
			container.Register<ICar, Audi>( "audi" );

			var car1 = container.GetInstance<ICar>( "bmw" );
			var car2 = container.GetInstance<ICar>( "bmw" );
			var car3 = container.GetInstance<ICar>( "audi" );

			Assert.AreSame( car1, car2 );
			Assert.AreNotSame( car1, car3 );
		}

		[Test]
		public void DoubleRegisterTypeWithNameTransient()
		{
			INDOContainer container = new NDOContainer();

			container.Register<ICar, BMW>( "bmw", new TransientLifetime() );
			container.Register<ICar, Audi>( "audi", new TransientLifetime() );

			var car1 = container.GetInstance<ICar>( "bmw" );
			var car2 = container.GetInstance<ICar>( "bmw" );
			var car3 = container.GetInstance<ICar>( "audi" );

			Assert.AreNotSame( car1, car2 );
			Assert.AreNotSame( car1, car3 );
		}

		[Test]
		public void ResolveOrRegisterTypeWithExplicitParameters()
		{
			INDOContainer container = new NDOContainer();
			var drv1 = container.ResolveOrRegisterType( ( f ) => new Driver( new BMW() ), "bmw" );
			var drv2 = container.ResolveOrRegisterType( ( f ) => new Driver( new Audi() ), "audi" );
			var drv3 = container.ResolveOrRegisterType( ( f ) => new Driver( new Hyunday(42) ), "hyundai" );
			Assert.AreEqual( "Running BMW - 1 mile", drv1.RunCar() );
			Assert.AreEqual( "Running Audi - 1 mile", drv2.RunCar() );
			Assert.AreEqual( "Running Hyunday - 43 miles", drv3.RunCar() );
			drv1 = container.ResolveOrRegisterType( ( f ) => new Driver( new BMW() ), "bmw" );
			drv2 = container.ResolveOrRegisterType( ( f ) => new Driver( new Audi() ), "audi" );
			drv3 = container.ResolveOrRegisterType( ( f ) => new Driver( new Hyunday( 42 ) ), "hyundai" ); 
			Assert.AreEqual( "Running BMW - 2 miles", drv1.RunCar() );
			Assert.AreEqual( "Running Audi - 2 miles", drv2.RunCar() );
			Assert.AreEqual( "Running Hyunday - 44 miles", drv3.RunCar() );
		}

		[Test]
		public void RegisterInstance()
		{
			INDOContainer container = new NDOContainer();

			var car1 = new BMW();
			container.RegisterInstance<ICar>( car1 );

			var car2 = container.GetInstance<ICar>();
			Assert.AreSame( car1, car2 );
		}

		[Test]
		public void RegisterInstanceWithName()
		{
			INDOContainer container = new NDOContainer();

			var bmw1 = new BMW();
			container.RegisterInstance<ICar>( bmw1, "bmw" );
			var audi1 = new Audi();
			container.RegisterInstance<ICar>( audi1, "audi" );

			var bmw2 = container.GetInstance<ICar>( "bmw" );
			Assert.AreSame( bmw1, bmw2 );
			var audi2 = container.GetInstance<ICar>( "audi" );
			Assert.AreSame( audi1, audi2 );
		}

		[Test]
		public void ResolveAndRegisterInstance()
		{
			INDOContainer container = new NDOContainer();
			var car1 = container.ResolveOrRegisterInstance<ICar>( "bmw", () => new BMW() );
			var car2 = container.ResolveOrRegisterInstance<ICar>( "bmw", () => new BMW() );
			Assert.AreSame( car1, car2 );

			var car3 = container.ResolveOrRegisterInstance<ICar>( "ford", () => new Ford() );
			Assert.AreNotSame( car1, car3 );

			var car4 = container.ResolveOrRegisterInstance<ICar>( "bmw2", () => new BMW() );
			Assert.AreNotSame( car1, car4 );
		}

		[Test]
		public void ChildContainerCanResolveParentRegistration()
		{
			INDOContainer container = new NDOContainer();
			container.Register<ICar, BMW>();

			var container2 = container.CreateChildContainer();
			
			var car = container2.Resolve<ICar>();
			Assert.IsNotNull( car );
			
		}

		[Test]
		public void ResolveAndRegisterInstanceWithFactory()
		{
			INDOContainer container = new NDOContainer();
			container.ResolveOrRegisterInstance( "bmw", () => new BMW() );
			var bmw = container.GetInstance( typeof( BMW ), "bmw" );
			Assert.IsNotNull( bmw );
			Assert.AreEqual( typeof( BMW ), bmw.GetType() );
			var bmw2 = container.GetInstance( typeof( BMW ), "bmw" );
			Assert.AreSame( bmw, bmw2 );
			container.ResolveOrRegisterInstance( "ford", () => new Ford() );
			var ford = container.GetInstance( typeof( Ford ), "ford" );
			Assert.That( ford is Ford );
		}


		[Test]
		public void ResolveWithChildContainer()
		{
			// This registration happens on static level
			INDOContainer container = new NDOContainer();

			container.Register<ICar, BMW>();
			var car1 = container.GetInstance<ICar>();
			var car2 = container.GetInstance<ICar>();
			Assert.AreSame( car1, car2 );

			using (var container2 = container.CreateChildContainer())
			{

				// This registration in the child container overwrites
				// the registration inherited from the parent container
				container2.Register<ICar, BMW>();  // PerScope
				car2 = container2.GetInstance<ICar>();
				Assert.AreNotSame( car1, car2 );  // Internal registration should override global reg.			
			}

			// static level remains as is
			car2 = container.GetInstance<ICar>();
			Assert.AreSame( car1, car2 );
		}

		[Test]
		public void ResolveWithChildContainer2()
		{
			INDOContainer container = new NDOContainer();
			container.Register<ICar, BMW>();
			container.Register<ICar, BMW>( "bmw" );

			var car1 = container.GetInstance<ICar>();
			ICar car2;

			using (var container2 = container.CreateChildContainer())
			{

				car2 = container2.GetInstance<ICar>();
				Assert.AreNotSame( car1, car2 );
				car2 = container2.GetInstance<ICar>( "bmw" );
				Assert.AreNotSame( car1, car2 );
			}

			// static level remains the same
			car2 = container.GetInstance<ICar>();
			Assert.AreSame( car1, car2 );
			car2 = container.GetInstance<ICar>( "bmw" );
			Assert.AreNotSame( car1, car2 );
		}

		[Test]
		public void ResolveInstanceWithChildContainer()
		{
			INDOContainer container1 = new NDOContainer();

			container1.RegisterInstance<ICar>( new BMW() );

			var car1 = container1.GetInstance<ICar>();
			var car2 = container1.GetInstance<ICar>();

			Assert.AreSame( car1, car2 );

			var container2 = container1.CreateChildContainer();

			container2.RegisterInstance<ICar>( new BMW() );
			car2 = container2.GetInstance<ICar>();

			Assert.AreNotSame( car1, car2 );

			container2.Dispose();

			car2 = container1.GetInstance<ICar>();
			Assert.AreSame( car1, car2 );
		}

		[Test]
		public void ResolveWithParameterInParentContainer()
		{
			var container1 = new NDOContainer();
			container1.RegisterInstance<ICar>( new BMW() );
			var bmw = container1.GetInstance<ICar>();
			var container2 = container1.CreateChildContainer();
			container2.Register<Driver>();
			var drv = container2.GetInstance<Driver>();
			Assert.NotNull( drv );
			Assert.AreEqual( "Running BMW - 1 mile", drv.RunCar() );

			container1 = new NDOContainer();
			container1.Register<Driver>();
			container2 = container1.CreateChildContainer();
			container2.RegisterInstance<ICar>( new BMW() );
			drv = container2.GetInstance<Driver>();
			Assert.NotNull( drv );
			Assert.AreEqual( "Running BMW - 1 mile", drv.RunCar() );
		}

		[Test]
		public void RegisterWithTransientLifetime()
		{
			INDOContainer container = new NDOContainer();
			container.Register<ICar, BMW>( new TransientLifetime() );
			container.Register<Driver>( new TransientLifetime() );

			var driver1 = container.GetInstance<Driver>();

			Assert.AreEqual( "Running BMW - 1 mile", driver1.RunCar() );

			var driver2 = container.GetInstance<Driver>();

			Assert.AreEqual( "Running BMW - 1 mile", driver2.RunCar() );
		}

		[Test]
		public void RegisterWithContainerLifetime()
		{
			var container = new ServiceContainer();
			container.Register<ICar, BMW>( new PerContainerLifetime() );

			var driver1 = container.Create<Driver>();
			Assert.AreEqual( "Running BMW - 1 mile", driver1.RunCar() );

			var driver2 = container.Create<Driver>();

			Assert.AreEqual( "Running BMW - 2 miles", driver2.RunCar() );
		}

		[Test]
		public void TestNullInstance()
		{
			var container = new ServiceContainer();
			container.Register<ICar>( _ => default( ICar ) );
			Assert.IsNull( container.GetInstance<ICar>() );
		}

		[Test]
		public void CanGetInstanceWorks()
		{
			var container = new ServiceContainer();
			Assert.AreEqual( false, container.CanGetInstance<ICar>() );
			container.RegisterInstance<ICar>( new BMW() );
			Assert.AreEqual( true, container.CanGetInstance<ICar>() );

			container = new ServiceContainer();
			container.Register<ICar, BMW>();
			Assert.AreEqual( true, container.CanGetInstance<ICar>() );
		}

		class Foo
		{
			public Foo() { }
			public Foo Bar { get; set; }
		}

		[Test]
		public void NDOContainerDisablesPropertyInjection()
		{
			// ServiceContainer allows Property Injection
			var liContainer = new ServiceContainer();
			liContainer.Register<Foo>();
			bool thrown = false;
			try
			{
				var foo = liContainer.GetInstance<Foo>();
			}
			catch (InvalidOperationException)
			{
				thrown = true;
			}
			// Recursive Resolve not allowed
			Assert.That( thrown );

			// NDOContainer doesn't allow Property Injection
			var ndoContainer = new NDOContainer();
			ndoContainer.Register<Foo>();
			thrown = false;
			try
			{
				var foo = ndoContainer.GetInstance<Foo>();
			}
			catch (InvalidOperationException)
			{
				thrown = true;
			}
			// Recursive Resolve possible, because PropertyInjection is disabled
			Assert.False( thrown );
		}


		[Test]
		public void OverwriteTypeRegWithInstanceReg()
		{
			// In LightInject overwriting is possible until the first GetInstance.
			// What we want is overwriting in the pm scope even after the first
			// GetInstance call in the static scope.
			// This is possible with child containers.
			var container = new NDOContainer();
			container.Register<ICar, BMW>();
			Assert.IsTrue( container.GetInstance<ICar>() is BMW );
			var container2 = container.CreateChildContainer();
			container2.RegisterInstance<ICar>( new Ford() );
			Assert.IsTrue( container2.GetInstance<ICar>() is Ford );
			Console.WriteLine();
		}

		[Test]
		public void OverwriteInstanceRegWithTypeReg()
		{
			var container = new NDOContainer();
			container.RegisterInstance<ICar>( new Ford() );
			Assert.IsTrue( container.GetInstance<ICar>() is Ford );
			var container2 = container.CreateChildContainer();
			container2.Register<ICar, BMW>();
			Assert.IsTrue( container2.GetInstance<ICar>() is BMW );
		}


		[Test]
		public void ResolveInterfaceWithoutRegistrationFails()
		{
			bool thrown = false;
			var container = new ServiceContainer();
			try
			{
				container.GetInstance<ICar>();
			}
			catch (InvalidOperationException)
			{
				thrown = true;
			}
			Assert.That( thrown );
		}

		[Test]
		public void ResolveTypeWithInstanceDependency()
		{
			bool thrown = false;
			try
			{
				INDOContainer container = new NDOContainer();
				container.RegisterInstance<ICar>( new BMW() );
				var drv = container.GetInstance<Driver>();  // Driver is not registered
			}
			catch (InvalidOperationException)
			{
				thrown = true;
			}
			Assert.That( thrown );
		}

	}
}
