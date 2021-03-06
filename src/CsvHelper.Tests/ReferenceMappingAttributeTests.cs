﻿// Copyright 2009-2011 Josh Close
// This file is a part of CsvHelper and is licensed under the MS-PL
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html
// http://csvhelper.com
using CsvHelper.Configuration;
using Moq;
using Xunit;

namespace CsvHelper.Tests
{
	public class ReferenceMappingAttributeTests
	{
		[Fact]
		public void ReferenceMappingTest()
		{
			var count = 0;
			var parserMock = new Mock<ICsvParser>();
			parserMock.Setup( m => m.Configuration ).Returns( new CsvConfiguration() );
			parserMock.Setup( m => m.Read() ).Returns( () =>
			{
				if( count == 0 )
				{
					return new[]
					{
						"FirstName",
						"LastName",
						"HomeStreet",
						"HomeCity",
						"HomeState",
						"HomeZip",
						"WorkStreet",
						"WorkCity",
						"WorkState",
						"WorkZip"
					};
				}
				count++;
				return new[]
				{
					"John",
					"Doe",
					"1234 Home St",
					"Home Town",
					"Home State",
					"12345",
					"5678 Work Rd",
					"Work City",
					"Work State",
					"67890"
				};
			} );

			var reader = new CsvReader( parserMock.Object );
			reader.Read();
			var person = reader.GetRecord<Person>();

			Assert.Equal( "FirstName", person.FirstName );
			Assert.Equal( "LastName", person.LastName );
			Assert.Equal( "HomeStreet", person.HomeAddress.Street );
			Assert.Equal( "HomeCity", person.HomeAddress.City );
			Assert.Equal( "HomeState", person.HomeAddress.State );
			Assert.Equal( "HomeZip", person.HomeAddress.Zip );
			Assert.Equal( "WorkStreet", person.WorkAddress.Street );
			Assert.Equal( "WorkCity", person.WorkAddress.City );
			Assert.Equal( "WorkState", person.WorkAddress.State );
			Assert.Equal( "WorkZip", person.WorkAddress.Zip );
		}

		private class Person
		{
			[CsvField( Name = "FirstName" )]
			public string FirstName { get; set; }

			[CsvField( Name = "LastName" )]
			public string LastName { get; set; }

			[CsvField( ReferenceKey = "Home" )]
			public Address HomeAddress { get; set; }

			[CsvField( ReferenceKey = "Work" )]
			public Address WorkAddress { get; set; }
		}

		private class Address
		{
			[CsvField( ReferenceKey = "Home", Name = "HomeStreet" )]
			[CsvField( ReferenceKey = "Work", Name = "WorkStreet" )]
			public string Street { get; set; }

			[CsvField( ReferenceKey = "Home", Name = "HomeCity" )]
			[CsvField( ReferenceKey = "Work", Name = "WorkCity" )]
			public string City { get; set; }

			[CsvField( ReferenceKey = "Home", Name = "HomeState" )]
			[CsvField( ReferenceKey = "Work", Name = "WorkState" )]
			public string State { get; set; }

			[CsvField( ReferenceKey = "Home", Name = "HomeZip" )]
			[CsvField( ReferenceKey = "Work", Name = "WorkZip" )]
			public string Zip { get; set; }
		}
	}
}
