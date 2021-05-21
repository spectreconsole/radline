using Shouldly;
using Xunit;

namespace RadLine.Tests
{
    public sealed class BackspaceCommandTests
    {
        [Fact]
        public void Should_Remove_Previous_Character()
        {
            // Given
            var buffer = new LineBuffer("Foo");
            var context = new LineEditorContext(buffer);
            var command = new BackspaceCommand();

            // When
            command.Execute(context);

            // Then
            buffer.Content.ShouldBe("Fo");
            buffer.Position.ShouldBe(2);
        }

        [Fact]
        public void Should_Do_Nothing_If_There_Is_Nothing_To_Remove()
        {
            // Given
            var buffer = new LineBuffer("Foo");
            buffer.Move(0);

            var context = new LineEditorContext(buffer);
            var command = new BackspaceCommand();

            // When
            command.Execute(context);

            // Then
            buffer.Content.ShouldBe("Foo");
            buffer.Position.ShouldBe(0);
        }
    }
}
