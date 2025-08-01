using System.Collections.Generic;
using DeliasWebsite.Core.Features.Search;
using Examine;
using Microsoft.AspNetCore.Http;
using Moq;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Models.PublishedContent;
using Xunit;

namespace DeliasWebsite.Core.Tests
{
    public class SearchServiceTests
    {
        [Fact]
        public void GetSearchResults_ReturnsEmpty_WhenQueryNullOrEmpty()
        {
            // Arrange
            var examineManagerMock = new Mock<IExamineManager>();
            var contextFactoryMock = new Mock<IUmbracoContextFactory>();
            contextFactoryMock.Setup(x => x.EnsureUmbracoContext())
                .Returns(default(UmbracoContextReference));
            var service = new SearchService(
                examineManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                contextFactoryMock.Object);

            // Act
            IEnumerable<IPublishedContent> resultNull = service.GetSearchResults(null);
            IEnumerable<IPublishedContent> resultEmpty = service.GetSearchResults(string.Empty);

            // Assert
            Assert.Empty(resultNull);
            Assert.Empty(resultEmpty);
        }
    }
}
