﻿using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Mustache.Reports.Domain.Options;
using Mustache.Reports.Domain.Report.Excel;
using Mustache.Reports.Domain.Report.Word;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace Mustache.Reports.Data.Tests
{
    public class ReportGatewayTests
    {
        [Fact]
        public void CreateWordReport_WhenValidInput_ShouldReturnRenderedReport()
        {
            //---------------Arrange------------------
            var configuration = SetupConfiguration();
            var reportData = File.ReadAllText("ExampleData\\WithImagesSampleData.json");
            var wordGateway = new ReportGateway(configuration);
            var input = new RenderWordInput {JsonModel = reportData, ReportName = "test.docx", TemplateName = "ReportWithImages" };
            //---------------Act----------------------
            var actual = wordGateway.CreateWordReport(input);
            //---------------Assert-------------------
            var expected = File.ReadAllText("Expected\\RenderedWordBase64.txt");
            Assert.Equal(expected.Substring(0,50), actual.Base64String.Substring(0,50));
        }

        [Fact]
        public void CreateExcelReport_WhenValidInput_ShouldReturnRenderedReport()
        {
            //---------------Arrange------------------
            var configuration = SetupConfiguration();
            var reportData = File.ReadAllText("ExampleData\\ExcelSampleData.json");
            var wordGateway = new ReportGateway(configuration);
            var input = new RenderExcelInput { JsonModel = reportData, ReportName = "test.xslx", TemplateName = "SimpleReport" };
            //---------------Act----------------------
            var actual = wordGateway.CreateExcelReport(input);
            //---------------Assert-------------------
            var expected = File.ReadAllText("Expected\\RenderedExcelBase64.txt");
            Assert.Equal(expected.Substring(0, 50), actual.Base64String.Substring(0, 50));
        }

        [Fact]
        public void CreateExcelReport_WhenInvalidTemplateName_ShouldReturnTemplateNameError()
        {
            //---------------Arrange------------------
            var configuration = SetupConfiguration();
            var reportGateway = new ReportGateway(configuration);
            var input = new RenderExcelInput { JsonModel = "", ReportName = "test.xslx", TemplateName = "INVALID_NAME" };
            //---------------Act----------------------
            var actual = reportGateway.CreateExcelReport(input);
            //---------------Assert-------------------
            Assert.True(actual.HasErrors());
            Assert.Contains("Invalid Report Template", actual.ErrorMessages[0]);
            Assert.Contains("INVALID_NAME.xlsx", actual.ErrorMessages[0]);
        }

        private static IOptions<MustacheReportOptions> SetupConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            string data = GetAppSettingJsonData();

            // ustacheReportOptions
            var reportOptions = JsonConvert.DeserializeObject<MustacheReportOptionsWrapper>(data);

            var result = Substitute.For<IOptions<MustacheReportOptions>>();
            result.Value.Returns(reportOptions.MustacheReportOptions);

            return result;
        }

        private static string GetAppSettingJsonData()
        {
            var filePath = Directory.GetCurrentDirectory();
            var settingsPath = Path.Combine(filePath, "appsettings.json");
            var data = File.ReadAllText(settingsPath);
            return data;
        }

        public class MustacheReportOptionsWrapper
        {
            public MustacheReportOptions MustacheReportOptions { get; set; }
        }
    }
}
