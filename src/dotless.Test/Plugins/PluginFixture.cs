﻿namespace dotless.Test.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core.Parser.Infrastructure;
    using Core.Parser.Tree;
    using Core.Plugins;
    using NUnit.Framework;
    using System.Globalization;
    using System.Threading;
    using System.ComponentModel;
    using System.Reflection;

    [DisplayName("Plugin A"), System.ComponentModel.Description("Plugs into A")]
    public class TestPlugin1 : IFunctionPlugin
    {
        public Dictionary<string, Type> GetFunctions()
        {
            return new Dictionary<string,Type>();
        }
    }

    [DisplayName("Plugin B"), System.ComponentModel.Description("Plugs into B")]
    public class TestPlugin2 : IFunctionPlugin
    {
        public TestPlugin2()
        {
        }

        public TestPlugin2(string one, int two, decimal three, bool four, double five)
        {
        }

        public Dictionary<string, Type> GetFunctions()
        {
            return new Dictionary<string,Type>();
        }
    }

    public class TestPluginConfiguratorB : GenericPluginConfigurator<TestPlugin2>
    {
    }

    public class PluginFixture : SpecFixtureBase
    {
        [Test]
        public void TestLoadingAndConfigurating()
        {
            IEnumerable<IPluginConfigurator> plugins = PluginFinder.GetConfigurators(Assembly.GetAssembly(typeof(PluginFixture)));

            Assert.AreEqual(2, plugins.Count());

            IPluginConfigurator plugin1 = plugins.ElementAt(0);
            IPluginConfigurator plugin2 = plugins.ElementAt(1);

            Assert.IsInstanceOf<GenericPluginConfigurator<TestPlugin1>>(plugin1);
            Assert.IsInstanceOf<TestPluginConfiguratorB>(plugin2);

            Assert.AreEqual("Plugin A", plugin1.Name);
            Assert.AreEqual("Plugin B", plugin2.Name);
            Assert.AreEqual("Plugs into A", plugin1.Description);
            Assert.AreEqual("Plugs into B", plugin2.Description);
        }

        [Test]
        public void TestGenericConfiguratorParams1()
        {
            IPluginConfigurator plugin1 = new GenericPluginConfigurator<TestPlugin1>();
            Assert.AreEqual(0, plugin1.GetParameters().Count());
            Assert.IsInstanceOf<TestPlugin1>(plugin1.CreatePlugin(plugin1.GetParameters()));
        }

        [Test]
        public void TestGenericConfiguratorParams2()
        {
            IPluginConfigurator plugin2 = new GenericPluginConfigurator<TestPlugin2>();
            var parameters = plugin2.GetParameters();
            Assert.AreEqual(5, parameters.Count());
            //string one, int two, decimal three, bool four, double five
            TestParam(parameters.ElementAt(0), "one", "String", false);
            TestParam(parameters.ElementAt(1), "two", "Int32", false);
            TestParam(parameters.ElementAt(2), "three", "Decimal", false);
            TestParam(parameters.ElementAt(3), "four", "Boolean", false);
            TestParam(parameters.ElementAt(4), "five", "Double", false);
            Assert.IsInstanceOf<TestPlugin2>(plugin2.CreatePlugin(new List<IPluginParameter>()));
        }

        private void TestParam(IPluginParameter param, string name, string typeDescription, bool isMandatory)
        {
            Assert.AreEqual(name, param.Name);
            Assert.AreEqual(typeDescription, param.TypeDescription);
            Assert.AreEqual(isMandatory, param.IsMandatory);
        }

        [Test]
        public void TestGenericConfiguratorParams3()
        {
            IPluginConfigurator plugin2 = new GenericPluginConfigurator<TestPlugin2>();
            var parameters = plugin2.GetParameters();
            //string one, int two, decimal three, bool four, double five
            parameters.ElementAt(0).SetValue("string");
            parameters.ElementAt(1).SetValue("2");
            parameters.ElementAt(2).SetValue("3.45");
            parameters.ElementAt(3).SetValue("true");
            parameters.ElementAt(4).SetValue("4.567");
            Assert.IsInstanceOf<TestPlugin2>(plugin2.CreatePlugin(parameters));
        }
    }
}
