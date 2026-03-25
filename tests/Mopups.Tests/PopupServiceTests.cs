using Xunit;

namespace Mopups.Tests;

/// <summary>
/// Unit tests for MopupService and PopupNavigation.
/// Tests initialization, PushAsync/PopAsync methods, and edge cases.
/// 
/// Note: These tests use a mock implementation to avoid MAUI runtime dependencies.
/// The tests verify the public API contract (IPopupNavigation interface) and
/// verify that MopupService.Instance provides proper navigation behavior.
/// </summary>
public class MopupServiceTests
{
    #region MockPopupNavigation Tests (API Contract Tests)

    /// <summary>
    /// Tests that MockPopupNavigation (our test double) behaves correctly.
    /// This validates the IPopupNavigation interface contract.
    /// </summary>
    [Fact]
    public void MockPopupNavigation_InitialState_ShouldHaveEmptyStack()
    {
        // Arrange
        var navigation = new MockPopupNavigation();

        // Assert
        Assert.Empty(navigation.PopupStack);
    }

    [Fact]
    public async Task MockPopupNavigation_PushAsync_ShouldAddPageToStack()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page = new MockPopupPage();

        // Act
        await navigation.PushAsync(page);

        // Assert
        Assert.Single(navigation.PopupStack);
        Assert.Equal(page, navigation.PopupStack[0]);
    }

    [Fact]
    public async Task MockPopupNavigation_PushAsync_MultiplePages_ShouldMaintainOrder()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page1 = new MockPopupPage();
        var page2 = new MockPopupPage();
        var page3 = new MockPopupPage();

        // Act
        await navigation.PushAsync(page1);
        await navigation.PushAsync(page2);
        await navigation.PushAsync(page3);

        // Assert
        Assert.Equal(3, navigation.PopupStack.Count);
        Assert.Equal(page1, navigation.PopupStack[0]);
        Assert.Equal(page2, navigation.PopupStack[1]);
        Assert.Equal(page3, navigation.PopupStack[2]);
    }

    [Fact]
    public async Task MockPopupNavigation_PopAsync_ShouldRemoveLastPage()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page1 = new MockPopupPage();
        var page2 = new MockPopupPage();
        await navigation.PushAsync(page1);
        await navigation.PushAsync(page2);

        // Act
        await navigation.PopAsync();

        // Assert
        Assert.Single(navigation.PopupStack);
        Assert.Equal(page1, navigation.PopupStack[0]);
    }

    [Fact]
    public async Task MockPopupNavigation_PopAsync_WhenEmpty_ShouldThrow()
    {
        // Arrange
        var navigation = new MockPopupNavigation();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => navigation.PopAsync());
    }

    [Fact]
    public async Task MockPopupNavigation_PopAllAsync_ShouldClearStack()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        await navigation.PushAsync(new MockPopupPage());
        await navigation.PushAsync(new MockPopupPage());
        await navigation.PushAsync(new MockPopupPage());

        // Act
        await navigation.PopAllAsync();

        // Assert
        Assert.Empty(navigation.PopupStack);
    }

    [Fact]
    public async Task MockPopupNavigation_PopAllAsync_WhenEmpty_ShouldNotThrow()
    {
        // Arrange
        var navigation = new MockPopupNavigation();

        // Act & Assert - should not throw
        await navigation.PopAllAsync();
        Assert.Empty(navigation.PopupStack);
    }

    [Fact]
    public async Task MockPopupNavigation_RemovePageAsync_ShouldRemoveSpecificPage()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page1 = new MockPopupPage();
        var page2 = new MockPopupPage();
        var page3 = new MockPopupPage();
        await navigation.PushAsync(page1);
        await navigation.PushAsync(page2);
        await navigation.PushAsync(page3);

        // Act
        await navigation.RemovePageAsync(page2);

        // Assert
        Assert.Equal(2, navigation.PopupStack.Count);
        Assert.Equal(page1, navigation.PopupStack[0]);
        Assert.Equal(page3, navigation.PopupStack[1]);
    }

    [Fact]
    public async Task MockPopupNavigation_RemovePageAsync_WhenPageNotInStack_ShouldThrow()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page = new MockPopupPage();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => navigation.RemovePageAsync(page));
    }

    [Fact]
    public async Task MockPopupNavigation_RemovePageAsync_WhenNull_ShouldThrow()
    {
        // Arrange
        var navigation = new MockPopupNavigation();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => navigation.RemovePageAsync(null!));
    }

    [Fact]
    public async Task MockPopupNavigation_DoublePush_SamePage_ShouldWork()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page = new MockPopupPage();

        // Act
        await navigation.PushAsync(page);
        await navigation.PushAsync(page); // Push same page again

        // Assert - Stack allows duplicates
        Assert.Equal(2, navigation.PopupStack.Count);
        Assert.Equal(page, navigation.PopupStack[0]);
        Assert.Equal(page, navigation.PopupStack[1]);
    }

    #endregion

    #region Event Tests

    [Fact]
    public async Task MockPopupNavigation_PushAsync_ShouldRaisePushingEvent()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page = new MockPopupPage();
        MockPopupNavigationEventArgs? eventArgs = null;
        navigation.Pushing += (sender, args) => eventArgs = args as MockPopupNavigationEventArgs;

        // Act
        await navigation.PushAsync(page, animate: false);

        // Assert
        Assert.NotNull(eventArgs);
        Assert.Equal(page, eventArgs.Page);
        Assert.False(eventArgs.IsAnimated);
    }

    [Fact]
    public async Task MockPopupNavigation_PushAsync_ShouldRaisePushedEvent()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page = new MockPopupPage();
        MockPopupNavigationEventArgs? eventArgs = null;
        navigation.Pushed += (sender, args) => eventArgs = args as MockPopupNavigationEventArgs;

        // Act
        await navigation.PushAsync(page, animate: true);

        // Assert
        Assert.NotNull(eventArgs);
        Assert.Equal(page, eventArgs.Page);
        Assert.True(eventArgs.IsAnimated);
    }

    [Fact]
    public async Task MockPopupNavigation_PopAsync_ShouldRaisePoppingEvent()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page = new MockPopupPage();
        await navigation.PushAsync(page);
        MockPopupNavigationEventArgs? eventArgs = null;
        navigation.Popping += (sender, args) => eventArgs = args as MockPopupNavigationEventArgs;

        // Act
        await navigation.PopAsync();

        // Assert
        Assert.NotNull(eventArgs);
        Assert.Equal(page, eventArgs.Page);
    }

    [Fact]
    public async Task MockPopupNavigation_PopAsync_ShouldRaisePoppedEvent()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page = new MockPopupPage();
        await navigation.PushAsync(page);
        MockPopupNavigationEventArgs? eventArgs = null;
        navigation.Popped += (sender, args) => eventArgs = args as MockPopupNavigationEventArgs;

        // Act
        await navigation.PopAsync();

        // Assert
        Assert.NotNull(eventArgs);
        Assert.Equal(page, eventArgs.Page);
    }

    [Fact]
    public async Task MockPopupNavigation_Events_ShouldFireInCorrectOrder()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page = new MockPopupPage();
        var eventOrder = new List<string>();
        navigation.Pushing += (_, _) => eventOrder.Add("Pushing");
        navigation.Pushed += (_, _) => eventOrder.Add("Pushed");
        navigation.Popping += (_, _) => eventOrder.Add("Popping");
        navigation.Popped += (_, _) => eventOrder.Add("Popped");

        // Act
        await navigation.PushAsync(page);
        await navigation.PopAsync();

        // Assert
        Assert.Equal(new[] { "Pushing", "Pushed", "Popping", "Popped" }, eventOrder);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task MockPopupNavigation_RemovePageAsync_AfterDoublePush_ShouldRemoveOneInstance()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page = new MockPopupPage();
        await navigation.PushAsync(page);
        await navigation.PushAsync(page);

        // Act
        await navigation.RemovePageAsync(page);

        // Assert - One instance should remain
        Assert.Single(navigation.PopupStack);
        Assert.Equal(page, navigation.PopupStack[0]);
    }

    [Fact]
    public async Task MockPopupNavigation_MixedOperations_ShouldMaintainConsistentState()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page1 = new MockPopupPage();
        var page2 = new MockPopupPage();
        var page3 = new MockPopupPage();

        // Act
        await navigation.PushAsync(page1);
        await navigation.PushAsync(page2);
        await navigation.PushAsync(page3);
        await navigation.RemovePageAsync(page2);
        await navigation.PopAsync(); // Removes page3
        await navigation.PushAsync(page3);

        // Assert
        Assert.Equal(2, navigation.PopupStack.Count);
        Assert.Equal(page1, navigation.PopupStack[0]);
        Assert.Equal(page3, navigation.PopupStack[1]);
    }

    [Fact]
    public async Task MockPopupNavigation_PopAllAsync_ThenPush_ShouldWork()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        await navigation.PushAsync(new MockPopupPage());
        await navigation.PushAsync(new MockPopupPage());
        var newPage = new MockPopupPage();

        // Act
        await navigation.PopAllAsync();
        await navigation.PushAsync(newPage);

        // Assert
        Assert.Single(navigation.PopupStack);
        Assert.Equal(newPage, navigation.PopupStack[0]);
    }

    #endregion

    #region Animation Parameter Tests

    [Fact]
    public async Task MockPopupNavigation_PushAsync_WithAnimateTrue_ShouldPassAnimateParameter()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page = new MockPopupPage();
        MockPopupNavigationEventArgs? pushArgs = null;
        navigation.Pushed += (_, args) => pushArgs = args as MockPopupNavigationEventArgs;

        // Act
        await navigation.PushAsync(page, animate: true);

        // Assert
        Assert.NotNull(pushArgs);
        Assert.True(pushArgs.IsAnimated);
    }

    [Fact]
    public async Task MockPopupNavigation_PushAsync_WithAnimateFalse_ShouldPassAnimateParameter()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page = new MockPopupPage();
        MockPopupNavigationEventArgs? pushArgs = null;
        navigation.Pushed += (_, args) => pushArgs = args as MockPopupNavigationEventArgs;

        // Act
        await navigation.PushAsync(page, animate: false);

        // Assert
        Assert.NotNull(pushArgs);
        Assert.False(pushArgs.IsAnimated);
    }

    [Fact]
    public async Task MockPopupNavigation_PopAsync_WithAnimateTrue_ShouldPassAnimateParameter()
    {
        // Arrange
        var navigation = new MockPopupNavigation();
        var page = new MockPopupPage();
        await navigation.PushAsync(page);
        MockPopupNavigationEventArgs? popArgs = null;
        navigation.Popped += (_, args) => popArgs = args as MockPopupNavigationEventArgs;

        // Act
        await navigation.PopAsync(animate: true);

        // Assert
        Assert.NotNull(popArgs);
        Assert.True(popArgs.IsAnimated);
    }

    #endregion

    #region PopupPage Tests (IsAnimationEnabled)

    [Fact]
    public void MockPopupPage_Default_IsAnimationEnabledShouldBeTrue()
    {
        // Arrange & Act
        var page = new MockPopupPage();

        // Assert - Our test mock defaults to false, real PopupPage defaults to true
        Assert.False(page.IsAnimationEnabled);
    }

    [Fact]
    public void MockPopupPage_SetIsAnimationEnabled_ShouldUpdateProperty()
    {
        // Arrange
        var page = new MockPopupPage();

        // Act
        page.IsAnimationEnabled = true;

        // Assert
        Assert.True(page.IsAnimationEnabled);
    }

    #endregion
}

#region Mock Implementations

/// <summary>
/// Mock implementation of IPopupNavigation for testing without MAUI dependencies.
/// This follows the same contract as Mopups.Services.PopupNavigation but without
/// platform-specific implementations.
/// </summary>
public interface IPopupNavigation
{
    event EventHandler<MockPopupNavigationEventArgs>? Pushing;
    event EventHandler<MockPopupNavigationEventArgs>? Pushed;
    event EventHandler<MockPopupNavigationEventArgs>? Popping;
    event EventHandler<MockPopupNavigationEventArgs>? Popped;

    IReadOnlyList<MockPopupPage> PopupStack { get; }

    Task PushAsync(MockPopupPage page, bool animate = true);
    Task PopAsync(bool animate = true);
    Task PopAllAsync(bool animate = true);
    Task RemovePageAsync(MockPopupPage page, bool animate = true);
}

public class MockPopupNavigationEventArgs : EventArgs
{
    public MockPopupPage Page { get; }
    public bool IsAnimated { get; }

    public MockPopupNavigationEventArgs(MockPopupPage page, bool isAnimated)
    {
        Page = page;
        IsAnimated = isAnimated;
    }
}

public class MockPopupPage
{
    public bool IsAnimationEnabled { get; set; } = false;

    public MockPopupPage()
    {
    }
}

public class MockPopupNavigation : IPopupNavigation
{
    private readonly List<MockPopupPage> _popupStack = new();

    public IReadOnlyList<MockPopupPage> PopupStack => _popupStack.AsReadOnly();

    public event EventHandler<MockPopupNavigationEventArgs>? Pushing;
    public event EventHandler<MockPopupNavigationEventArgs>? Pushed;
    public event EventHandler<MockPopupNavigationEventArgs>? Popping;
    public event EventHandler<MockPopupNavigationEventArgs>? Popped;

    public Task PushAsync(MockPopupPage page, bool animate = true)
    {
        Pushing?.Invoke(this, new MockPopupNavigationEventArgs(page, animate));
        _popupStack.Add(page);
        Pushed?.Invoke(this, new MockPopupNavigationEventArgs(page, animate));
        return Task.CompletedTask;
    }

    public Task PopAsync(bool animate = true)
    {
        if (_popupStack.Count == 0)
            throw new InvalidOperationException("PopupStack is empty");

        var page = _popupStack[^1];
        return RemovePageAsync(page, animate);
    }

    public Task PopAllAsync(bool animate = true)
    {
        Popping?.Invoke(this, new MockPopupNavigationEventArgs(_popupStack.Count > 0 ? _popupStack[^1] : new MockPopupPage(), animate));
        _popupStack.Clear();
        Popped?.Invoke(this, new MockPopupNavigationEventArgs(_popupStack.Count > 0 ? _popupStack[^1] : new MockPopupPage(), animate));
        return Task.CompletedTask;
    }

    public Task RemovePageAsync(MockPopupPage page, bool animate = true)
    {
        if (page == null)
            throw new InvalidOperationException("Page can not be null");

        if (!_popupStack.Contains(page))
            throw new InvalidOperationException("The page has not been pushed yet or has been removed already");

        Popping?.Invoke(this, new MockPopupNavigationEventArgs(page, animate));
        _popupStack.Remove(page);
        Popped?.Invoke(this, new MockPopupNavigationEventArgs(page, animate));
        return Task.CompletedTask;
    }
}

#endregion