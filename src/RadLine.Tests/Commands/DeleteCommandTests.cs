using Shouldly;
using Xunit;

namespace RadLine.Tests
{
    public sealed class DeleteCommandTests
    {
        [Fact]
        public void Should_Delete_Next_Character()
        {
            // Given
            var buffer = new LineBuffer("Foo");
            buffer.Move(0);

            var context = new LineEditorContext(buffer);
            var command = new DeleteCommand();

            // When
            command.Execute(context);

            // Then
            buffer.Content.ShouldBe("oo");
            buffer.Position.ShouldBe(0);
        }

        [Fact]
        public void Should_Do_Nothing_If_There_Is_Nothing_To_Remove()
        {
            // Given
            var buffer = new LineBuffer("Foo");
            var context = new LineEditorContext(buffer);
            var command = new DeleteCommand();

            // When
            command.Execute(context);

            // Then
            buffer.Content.ShouldBe("Foo");
            buffer.Position.ShouldBe(3);
        }
    }
}
