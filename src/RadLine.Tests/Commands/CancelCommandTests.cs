using Shouldly;
using Xunit;

namespace RadLine.Tests
{
    public sealed class CancelCommandTests
    {
        [Fact]
        public void Should_Cancel_Input()
        {
            // Given
            var buffer = new LineBuffer("Foo");
            var context = new LineEditorContext(buffer);
            buffer.Insert("Bar");
            var command = new CancelCommand();

            // When
            command.Execute(context);

            // Then
            buffer.Content.ShouldBe("Foo");
            buffer.Position.ShouldBe(3);
        }
    }
}
