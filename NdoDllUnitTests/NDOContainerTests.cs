﻿using NUnit.Framework;
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
		public void ResolveWithNullName()
		{
			// Resolve with serviceName == null is OK, 
			INDOContainer container = new NDOContainer();
			container.Register<ICar, BMW>();
			var car = container.GetInstance<ICar>( null );
			Assert.IsNotNull( car );

			// GetInstance with serviceName == null is not OK
			bool thrown = false;
			try
			{
				car = container.GetInstance<ICar>( null );
			}
			catch (ArgumentNullException)
			{
				thrown = true;
			}
			Assert.That( thrown );
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
			using (container.BeginScope())
			{
				var car1 = container.GetInstance<ICar>();
				var car2 = container.GetInstance<ICar>();
				Assert.AreSame( car1, car2 );
			}
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
		public void ResolveOrRegisterTypeWithOverride()
		{
#warning If we need GetInstanceOrRegisterType we could write an extension using TryGetInstance<T>.
			//IServiceContainer container = new ServiceContainer();

			//var drv1 = container.GetInstanceOrRegisterType<Driver>(new PerContainerLifetime(), "bmw", new ParameterOverride("car", new BMW()));
			//var drv2 = container.GetInstanceOrRegisterType<Driver>(new PerContainerLifetime(), "audi", new ParameterOverride("car", new Audi()));
			//Assert.AreEqual( "Running BMW - 1 mile", drv1.RunCar() );
			//Assert.AreEqual( "Running Audi - 1 mile", drv2.RunCar() );
			//drv1 = container.GetInstanceOrRegisterType<Driver>( new PerContainerLifetime(), "bmw", new ParameterOverride( "car", new BMW() ) );
			//drv2 = container.GetInstanceOrRegisterType<Driver>( new PerContainerLifetime(), "audi", new ParameterOverride( "car", new Audi() ) );
			//Assert.AreEqual( "Running BMW - 2 mile", drv1.RunCar() );
			//Assert.AreEqual( "Running Audi - 2 mile", drv2.RunCar() );
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
			using (container.BeginScope())
			{
				var car1 = container.ResolveOrRegisterInstance<ICar>( "bmw", () => new BMW() );
				var car2 = container.ResolveOrRegisterInstance<ICar>( "bmw", () => new BMW() );
				Assert.AreSame( car1, car2 );

				var car3 = container.ResolveOrRegisterInstance<ICar>( "ford", () => new Ford() );
				Assert.AreNotSame( car1, car3 );

				var car4 = container.ResolveOrRegisterInstance<ICar>( "bmw2", () => new BMW() );
				Assert.AreNotSame( car1, car4 );
			}
		}

		[Test]
		public void NestedScopeCanResolveGlobalRegistration()
		{
			INDOContainer container = new NDOContainer();
			container.Register<ICar, BMW>();

			using (container.BeginScope())
			{
				var car = container.Resolve<ICar>();
				Assert.IsNotNull( car );
			}
		}

		[Test]
		public void ChildContainerCanResolveGlobalRegistration()
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

			// This registration happens on pm instance level
			var container2 = container.CreateChildContainer();
			car2 = container2.GetInstance<ICar>();  // We have no registration at this point
			Assert.AreSame( car1, car2 );			// so we get the object from parent container

			container2.Register<ICar, BMW>();  // PerScope
			car2 = container2.GetInstance<ICar>();

			Assert.AreNotSame( car1, car2 );  // Internal registration should override global reg.			

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

			var container2 = container.CreateChildContainer();
			
			car2 = container2.GetInstance<ICar>();
			Assert.AreSame( car1, car2 );
			car2 = container2.GetInstance<ICar>( "bmw" );
			Assert.AreNotSame( car1, car2 );
			
			// static level remains the same
			car2 = container.GetInstance<ICar>();
			Assert.AreSame( car1, car2 );
			car2 = container.GetInstance<ICar>( "bmw" );
			Assert.AreNotSame( car1, car2 );
		}

		[Test]
		public void ResolveInstanceChildContainer()
		{
			INDOContainer container = new NDOContainer();
			container.Register<ICar>( _ => new BMW(), new TransientLifetime() );

			var car1 = container.GetInstance<ICar>();
			ICar car2;

			var container2 = container.CreateChildContainer();
			car2 = container2.GetInstance<ICar>();
			Assert.AreSame( car1, car2 );

			// Registration can't be overridden after GetInstance
			container2.Register<ICar>( _ => new Ford(), new PerScopeLifetime() );
			car2 = container2.GetInstance<ICar>();
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

		//[Test]
		//public void RegisterWithTransientLifetime()
		//{
		//	var container = new ServiceContainer();
		//	container.Register<ICar, BMW>( new TransientLifetimeManager() );

		//	var driver1 = container.GetInstance<Driver>();

		//	Assert.AreEqual( "Running BMW - 1 mile", driver1.RunCar() );

		//	var driver2 = container.GetInstance<Driver>();

		//	Assert.AreEqual( "Running BMW - 1 mile", driver2.RunCar() );
		//}

		//[Test]
		//public void RegisterWithContainerLifetime()
		//{
		//	var container = new ServiceContainer();
		//	container.Register<ICar, BMW>( new PerContainerLifetime() );

		//	var driver1 = container.GetInstance<Driver>();
		//	Assert.AreEqual( "Running BMW - 1 mile", driver1.RunCar() );

		//	var driver2 = container.GetInstance<Driver>();

		//	Assert.AreEqual( "Running BMW - 2 mile", driver2.RunCar() );
		//}

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
		public void CanUseTransientLifetime()
		{
			var container = new NDOContainer();
			container.Register<ICar, BMW>( new TransientLifetime() );
			var car1 = container.GetInstance<ICar>();
			var car2 = container.GetInstance<ICar>();
			Assert.AreNotSame( car1, car2 );
		}

		[Test]
		public void OverwriteTypeRegWithInstanceReg()
		{
			// Overwriting is possible until the first GetInstance.
			// What we want is overwriting in the pm scope even after the first GetInstance call.
			// This is only possible with child containers.
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
		public void ResolveInterfaceWithoutRegistrationIsNull()
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
				var container = new ServiceContainer();
				container.RegisterInstance<ICar>( new BMW() );
				var drv = container.GetInstance<Driver>();  // Driver is not registered
				Assert.NotNull( drv );
			}
			catch (InvalidOperationException)
			{
				thrown = true;
			}
			Assert.That( thrown );
		}

	}
}
