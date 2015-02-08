using System;
using SFRestApiUpdater.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;

namespace SFRestApiUpdater.Tests
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void TestToJsonString()
        {
            String expectedJson = "{\"Id\":1,\"Name\":\"John Doe\"}";
            Object jsonObject = new { Id = 1, Name = "John Doe" };
            String actualJson = jsonObject.ToJsonString();
            Assert.AreEqual(expectedJson, actualJson);
        }

        [TestMethod]
        public void TestToDynamicObject()
        {
            dynamic expectedObject = new ExpandoObject();
            expectedObject.Id=1;
            expectedObject.Name="John Doe";
            dynamic actualObject = new SimpleTestObject().ToDynamic();
            Assert.AreEqual(expectedObject.Id, actualObject.Id);
            Assert.AreEqual(expectedObject.Name, actualObject.Name);
        }

        [TestMethod]
        public void TestToDynamicObjectWithAttributes()
        {
            dynamic expectedObject = new ExpandoObject();
            expectedObject.Id = 1;
            expectedObject.Name = "John Doe";

            dynamic actualObject = new AttributeDecoratedTestObject().ToDynamic();
            Assert.AreEqual(expectedObject.Id, actualObject.Id__c);
            Assert.AreEqual(expectedObject.Name, actualObject.Name__c);

            try {
                var i = actualObject.AlternativeId;
                Assert.Fail("RuntimeBinderException should have been thrown as Ignore flag on Salesforce Attribute was set.");
            } catch (Exception ex)
            { Assert.IsTrue(ex is RuntimeBinderException); }

        }
    }

    /*
     * Test Objects
     */

    class SimpleTestObject
    {
        public int Id { get; set; }
        public String Name { get; set; }
        
        public SimpleTestObject()
        {
            Id = 1;
            Name = "John Doe";
        }
    }

    class AttributeDecoratedTestObject
    {
        [SalesforceAttribute(ApiName="Id__c")]
        public int Id { get; set; }

        [SalesforceAttribute(ApiName = "Name__c")]
        public String Name { get; set; }

        [SalesforceAttribute(Ignore=true)]
        public int AlternativeID { get; set; }

        public AttributeDecoratedTestObject()
        {
            Id = 1;
            Name = "John Doe";
            AlternativeID = 100;
        }
    }
}
