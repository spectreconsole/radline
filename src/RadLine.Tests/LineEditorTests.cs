using System;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Spectre.Console.Testing;
using Xunit;

namespace RadLine.Tests
{
    public sealed class LineEditorTests
    {
        [Fact]
        public async Task Should_Return_Entered_Text_When_Pressing_Enter()
        {
            // Given
            var editor = new LineEditor(
                new TestConsole(),
                new TestInputSource()
                    .Push("Patrik")
                    .Push(ConsoleKey.Enter));

            // When
            var result = await editor.ReadLine(CancellationToken.None);

            // Then
            result.ShouldBe("Patrik");
        }

        [Fact]
        public async Task Should_Add_New_Line_When_Pressing_Shift_And_Enter()
        {
            // Given
            var editor = new LineEditor(
                new TestConsole(),
                new TestInputSource()
                    .Push("Patrik")
                    .Push(ConsoleKey.Enter, ConsoleModifiers.Shift)
                    .Push("Svensson")
                    .Push(ConsoleKey.Enter))
            {
                MultiLine = true,
            };

            // When
            var result = await editor.ReadLine(CancellationToken.None);

            // Then
            result.ShouldBe($"Patrik{Environment.NewLine}Svensson");
        }

        [Fact]
        public async Task Should_Move_To_Previous_Item_In_History()
        {
            // Given
            var editor = new LineEditor(
                new TestConsole(),
                new TestInputSource()
                    .Push(ConsoleKey.UpArrow, ConsoleModifiers.Control)
                    .Push(ConsoleKey.UpArrow, ConsoleModifiers.Control)
                    .Push(ConsoleKey.UpArrow, ConsoleModifiers.Control)
                    .Push(ConsoleKey.Enter));

            editor.History.Add("Foo");
            editor.History.Add("Bar");
            editor.History.Add("Baz");

            // When
            var result = await editor.ReadLine(CancellationToken.None);

            // Then
            result.ShouldBe("Foo");
        }

        [Fact]
        public async Task Should_Move_To_Next_Item_In_History()
        {
            // Given
            var editor = new LineEditor(
                new TestConsole(),
                new TestInputSource()
                    .Push(ConsoleKey.UpArrow, ConsoleModifiers.Control)
                    .Push(ConsoleKey.UpArrow, ConsoleModifiers.Control)
                    .Push(ConsoleKey.UpArrow, ConsoleModifiers.Control)
                    .Push(ConsoleKey.DownArrow, ConsoleModifiers.Control)
                    .Push(ConsoleKey.DownArrow, ConsoleModifiers.Control)
                    .Push(ConsoleKey.Enter));

            editor.History.Add("Foo");
            editor.History.Add("Bar");
            editor.History.Add("Baz");

            // When
            var result = await editor.ReadLine(CancellationToken.None);

            // Then
            result.ShouldBe("Baz");
        }

        [Fact]
        public async Task Should_Add_Entered_Text_To_History()
        {
            // Given
            var input = new TestInputSource();
            var editor = new LineEditor(new TestConsole(), input);
            input.Push("Patrik").Push(ConsoleKey.Enter);
            await editor.ReadLine(CancellationToken.None);

            // When
            input.Push(ConsoleKey.UpArrow, ConsoleModifiers.Control).Push(ConsoleKey.Enter);
            var result = await editor.ReadLine(CancellationToken.None);

            // Then
            result.ShouldBe("Patrik");
        }
    }
}
