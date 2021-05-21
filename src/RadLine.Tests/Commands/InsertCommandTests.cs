using Shouldly;
using Xunit;

namespace RadLine.Tests
{
    public sealed class InsertCommandTests
    {
        [Fact]
        public void Should_Insert_Text_At_Position()
        {
            // Given
            var buffer = new LineBuffer("Foo");
            var context = new LineEditorContext(buffer);
            var command = new InsertCommand('l');

            // When
            command.Execute(context);

            // Then
            buffer.Content.ShouldBe("Fool");
            buffer.Position.ShouldBe(4);
        }
    }
}
