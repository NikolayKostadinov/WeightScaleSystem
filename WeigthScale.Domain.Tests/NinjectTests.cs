using System;
using System.Reflection;
using System.Web.Http.Tracing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using WeightScale.Application.AppStart;
using WeightScale.Application.Contracts;
using WeightScale.Application.Services;

namespace WeigthScale.Domain.Tests
{
    [TestClass]
    public class NinjectTests
    {
        private static readonly IKernel kernel = NinjectInjector.GetInjector;

        [TestMethod]
        public void BindILogWhenTypeIsCustomeWriter()
        {
            // Arrange
            

            // Act
            var aClass = kernel.Get<ITraceWriter>();

            // Assert
            FieldInfo field = aClass.GetType().GetField("logger", BindingFlags.NonPublic |BindingFlags.GetField | BindingFlags.Instance);
            object objx = field.GetValue(aClass);
            var logWriterLogger = objx.GetType().BaseType.GetField("m_logger",BindingFlags.NonPublic |BindingFlags.GetField | BindingFlags.Instance);
            var logWriterLoggerValue = logWriterLogger.GetValue(objx);
            var loggerName = logWriterLoggerValue.GetType().BaseType.GetField("m_name", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var loggerNameValue = loggerName.GetValue(logWriterLoggerValue);

            Assert.AreEqual("WebApiTrace", loggerNameValue);
        }

        [TestMethod]
        public void BindILogWhenTypeIsNotCustomeWriter()
        {
            // Arrange

            // Act
            var aClass = kernel.Get<IFileService>();

            // Assert
            FieldInfo field = aClass.GetType().GetField("logger", BindingFlags.NonPublic |BindingFlags.GetField | BindingFlags.Instance);
            object objx = field.GetValue(aClass);
            var logWriterLogger = objx.GetType().BaseType.GetField("m_logger",BindingFlags.NonPublic |BindingFlags.GetField | BindingFlags.Instance);
            var logWriterLoggerValue = logWriterLogger.GetValue(objx);
            var loggerName = logWriterLoggerValue.GetType().BaseType.GetField("m_name", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var loggerNameValue = loggerName.GetValue(logWriterLoggerValue);

            Assert.AreNotEqual("WebApiTrace", loggerNameValue);
        }
    }
}
