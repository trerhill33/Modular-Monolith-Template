using ModularTemplate.Common.Application.Features;
using ModularTemplate.Common.Domain.Results;
using Xunit;

namespace ModularTemplate.Common.Application.Tests.Features;

public class FeatureErrorsTests
{
    [Fact]
    public void FeatureDisabled_ReturnsCorrectError()
    {
        var error = FeatureErrors.FeatureDisabled("MyFeature");

        Assert.Equal("Feature.Disabled", error.Code);
        Assert.Contains("MyFeature", error.Description);
        Assert.Equal(ErrorType.Failure, error.Type);
    }
}
