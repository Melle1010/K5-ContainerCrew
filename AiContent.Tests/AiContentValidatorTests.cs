using AI_Content_Assistant.Exceptions;
using AI_Content_Assistant.Validators;
using System.Linq;
using Xunit;

namespace AiContent.Tests
{
    public class AiContentValidatorTests
    {
        [Fact]
        public void Validate_EmptyString_Throws()
        {
            Assert.Throws<AiContentQualityException>(() => AiContentValidator.Validate(""));
        }

        [Fact]
        public void Validate_Whitespace_Throws()
        {
            Assert.Throws<AiContentQualityException>(() => AiContentValidator.Validate("   "));
        }

        [Fact]
        public void Validate_TooManySentences_Throws()
        {
            var text = string.Join(" ", new string[11].Select(_ => "Sentence."));
            Assert.Throws<AiContentQualityException>(() => AiContentValidator.Validate(text));
        }

        [Fact]
        public void Validate_ValidText_DoesNotThrow()
        {
            var text = "This is one sentence. This is two! Is this three?";
            var ex = Record.Exception(() => AiContentValidator.Validate(text));
            Assert.Null(ex);
        }
    }
}
