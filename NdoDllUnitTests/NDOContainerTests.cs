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
			IServiceContainer container = new ServiceContainer();

			container.Register<ICar, BMW>();
			container.Register<Driver>();

			//Resolves dependencies and returns the Driver object 
			Driver drv = container.GetInstance<Driver>();
			Assert.AreEqual( "Running BMW - 1 mile", drv.RunCar() );
		}

		[Test]
		public void SimpleRegistrationWithOneType()
		{
			IServiceContainer container = new ServiceContainer();

			container.Register<BMW>();

			var car = container.GetInstance<BMW>();
			Assert.AreEqual( typeof( BMW ), car.GetType() );
		}

		[Test]
		public void GetUnregistered()
		{
			IServiceContainer container = new ServiceContainer();
			var car = container.TryGetInstance<ICar>();
			Assert.IsNull( car );
		}

		[Test]
		public void SimpleRegistrationWithOverride()
		{
			IServiceContainer container = new ServiceContainer();

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
			IServiceContainer container = new ServiceContainer();

			container.Register<ICar, BMW>(new PerScopeLifetime());
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
			IServiceContainer container = new ServiceContainer();

			container.Register<ICar, BMW>( "car1", new PerContainerLifetime() );
			container.Register<ICar, BMW>( "car2", new PerContainerLifetime() );

			var car1 = container.GetInstance<ICar>("car1");
			var car2 = container.GetInstance<ICar>("car1");
			var car3 = container.GetInstance<ICar>("car2");

			Assert.AreSame( car1, car2 );
			Assert.AreNotSame( car1, car3 );
		}

		[Test]
		public void DoubleRegisterTypeWithName()
		{
			IServiceContainer container = new ServiceContainer();

			container.Register<ICar, BMW>( "bmw", new PerContainerLifetime() );
			container.Register<ICar, Audi>( "audi", new PerContainerLifetime() );

			var car1 = container.GetInstance<ICar>( "bmw" );
			var car2 = container.GetInstance<ICar>( "bmw" );
			var car3 = container.GetInstance<ICar>( "audi" );

			Assert.AreSame( car1, car2 );
			Assert.AreNotSame( car1, car3 );
		}

		[Test]
		public void DoubleRegisterTypeWithNameTransient()
		{
			IServiceContainer container = new ServiceContainer();

			container.Register<ICar, BMW>("bmw");
			container.Register<ICar, Audi>("audi");

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
			IServiceContainer container = new ServiceContainer();

			var car1 = new BMW();
			container.RegisterInstance<ICar>( car1 );

			var car2 = container.GetInstance<ICar>();
			Assert.AreSame( car1, car2 );
		}

		[Test]
		public void RegisterInstanceWithName()
		{
			IServiceContainer container = new ServiceContainer();

			var bmw1 = new BMW();
			container.RegisterInstance<ICar>( bmw1, "bmw" );
			var audi1 = new Audi();
			container.RegisterInstance<ICar>( audi1, "audi" );

			var bmw2 = container.GetInstance<ICar>("bmw");
			Assert.AreSame( bmw1, bmw2 );
			var audi2 = container.GetInstance<ICar>("audi");
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
		public void Foo()
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
		public void ResolveAndRegisterInstanceWithFactory()
		{
			INDOContainer container = new NDOContainer();
			using (container.BeginScope())
			{
				container.ResolveOrRegisterInstance( "bmw", () => new BMW() );
				var bmw = container.GetInstance( typeof( BMW ), "bmw" );
				Assert.IsNotNull( bmw );
				Assert.AreEqual( typeof( BMW ), bmw.GetType() );
				var bmw2 = container.GetInstance( typeof( BMW ), "bmw" );
				Assert.AreSame( bmw, bmw2 );
			}
		}


		[Test]
		public void ResolveWithNestedScopes()
		{
			// This registration happens on static level
			IServiceContainer container = new ServiceContainer();
			
			using (container.BeginScope())
			{
				container.Register<ICar, BMW>( new PerScopeLifetime() );
				var car1 = container.GetInstance<ICar>();
				var car2 = container.GetInstance<ICar>();
				Assert.AreSame( car1, car2 );

				// This registration happens on pm instance level
				using (container.BeginScope())
				{
					container.Register<ICar, BMW>( new PerScopeLifetime() );
					car2 = container.GetInstance<ICar>();

					Assert.AreNotSame( car1, car2 );

					//container2.Dispose();

					//car2 = container1.Resolve<ICar>();
					//Assert.AreSame( car1, car2 );
				}

				// Back in static level
				car2 = container.GetInstance<ICar>();
				Assert.AreSame( car1, car2 );
			}
		}

		[Test]
		public void ResolveWithNestedScope2()
		{
			IServiceContainer container = new ServiceContainer();
			using (container.BeginScope())
			{
				container.Register<ICar, BMW>( new PerScopeLifetime() );
				container.Register<ICar, BMW>( "bmw", new PerScopeLifetime() );

				var car1 = container.GetInstance<ICar>();
				ICar car2;

				using (container.BeginScope())
				{
					car2 = container.GetInstance<ICar>();

					Assert.AreNotSame( car1, car2 );

					container.Register<ICar, BMW>();
					car2 = container.GetInstance<ICar>();

					Assert.AreNotSame( car1, car2 );

				}

				car2 = container.GetInstance<ICar>();
				Assert.AreSame( car1, car2 );
				car2 = container.GetInstance<ICar>( "bmw" );
				Assert.AreNotSame( car1, car2 );
			}
		}

		[Test]
		public void ResolveInstanceNestedScope()
		{
			IServiceContainer container = new ServiceContainer();
			using (container.BeginScope())
			{
				container.Register<ICar>( _=> new BMW(), new PerContainerLifetime() );

				var car1 = container.GetInstance<ICar>();
				ICar car2;

				using (container.BeginScope())
				{
					car2 = container.GetInstance<ICar>();

					Assert.AreSame( car1, car2 );

					// PerContainer Registration can't be overridden
					container.Register<ICar>( _=> new BMW(), new PerScopeLifetime() );
					car2 = container.GetInstance<ICar>();
					Assert.AreSame( car1, car2 );

					// If we want to override a Registration in the inner scope
					// we have to use names
					container.Register<ICar>(  _ => new BMW(), "bmw", new PerScopeLifetime() );
					car2 = container.GetInstance<ICar>("bmw");
					Assert.AreNotSame( car1, car2 );
				}

				car2 = container.GetInstance<ICar>();
				Assert.AreSame( car1, car2 );
			}
		}

		[Test]
		public void ResolveWithParameterInParentContainer()
		{
			var container = new ServiceContainer();
			container.SetDefaultLifetime<PerScopeLifetime>();
			
			using (container.BeginScope())
			{
				container.Register<Driver>();
				using (container.BeginScope())
				{
					container.RegisterInstance<ICar>( new BMW() );
					var drv = container.GetInstance<Driver>();
					Assert.NotNull( drv );
					Assert.AreEqual( "Running BMW - 1 mile", drv.RunCar() );
				}
			}

			container = new ServiceContainer();
			container.SetDefaultLifetime<PerScopeLifetime>();

			using (container.BeginScope())
			{
				container.RegisterInstance<ICar>( new BMW() );
				using (container.BeginScope())
				{
					container.Register<Driver>();
					var drv = container.GetInstance<Driver>();
					Assert.NotNull( drv );
					Assert.AreEqual( "Running BMW - 1 mile", drv.RunCar() );
				}
			}
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
			container.Register<ICar>( _ => default(ICar) );
			Assert.IsNull( container.GetInstance<ICar>() );
		}

		[Test]
		public void IsRegisteredWorks()
		{
			var container = new ServiceContainer();
			Assert.AreEqual( false, container.CanGetInstance<ICar>() );
			container.RegisterInstance<ICar>( new BMW() );
			Assert.AreEqual( true, container.CanGetInstance<ICar>() );

			container = new ServiceContainer();
			container.Register<ICar, BMW>();
			Assert.AreEqual( true, container.CanGetInstance<ICar>() );
		}


		[Test]
		public void OverwriteTypeRegWithInstanceReg()
		{
			var container = new ServiceContainer();
			container.SetDefaultLifetime<PerScopeLifetime>();
			using (container.BeginScope())
			{
				container.Register<ICar, BMW>();
				container.RegisterInstance<ICar>( new Ford() );
				Assert.IsTrue( container.GetInstance<ICar>() is Ford );
			}
		}

		[Test]
		public void OverwriteInstanceRegWithTypeReg()
		{
			var container = new ServiceContainer();
			container.RegisterInstance<ICar>( new Ford() );
			container.Register<ICar, BMW>();
			Assert.IsTrue( container.GetInstance<ICar>() is BMW );
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
