# WPF Interview Questions for Senior Architect (13 Years Experience)

## 1. MVVM Pattern & Architecture

### Q1: Explain the MVVM pattern and why it's preferred in WPF applications
**Answer:**
- Model-View-ViewModel pattern separates UI (View) from business logic (ViewModel) and data (Model)
- View contains only XAML with minimal code-behind
- ViewModel exposes data and commands through INotifyPropertyChanged
- Model contains business logic and data
- Benefits: testability, maintainability, separation of concerns, designer-developer workflow
- Data binding connects View to ViewModel without direct references

### Q2: How does INotifyPropertyChanged work and why is it important?
**Answer:**
- Interface that notifies UI when property values change
- Raises PropertyChanged event with property name
- WPF binding system subscribes to these events
- Without it, UI won't update when ViewModel changes
- Modern approach uses CallerMemberName attribute
- Can be optimized with SetProperty helper method

### Q3: What are the differences between ObservableCollection and List?
**Answer:**
- ObservableCollection implements INotifyCollectionChanged
- Notifies UI when items are added, removed, or moved
- List doesn't notify, requiring full collection replacement
- ObservableCollection is thread-unsafe (use BindingOperations.EnableCollectionSynchronization)
- Performance: List is faster for large operations, but requires UI refresh
- Use ObservableCollection for UI-bound collections

## 2. Data Binding

### Q4: Explain the different binding modes in WPF
**Answer:**
- **OneWay**: Source ? Target only (default for most properties)
- **TwoWay**: Source ? Target bidirectional (default for TextBox.Text)
- **OneTime**: Set once, no updates
- **OneWayToSource**: Target ? Source only
- **Default**: Uses property's default binding mode
- UpdateSourceTrigger controls when updates occur (PropertyChanged, LostFocus, Explicit)

### Q5: What is the difference between StaticResource and DynamicResource?
**Answer:**
- **StaticResource**: Resolved once at XAML load, better performance
- **DynamicResource**: Re-evaluated when resource changes, supports runtime updates
- StaticResource throws exception if not found
- DynamicResource uses default value if not found
- Use StaticResource by default, DynamicResource for theming
- DynamicResource has performance overhead

### Q6: How do you implement master-detail scenarios in WPF?
**Answer:**
- Bind master collection to ItemsControl (ListBox, DataGrid)
- Bind detail view to SelectedItem property
- Use IsSynchronizedWithCurrentItem for collection views
- CollectionView provides CurrentItem for navigation
- Can use ICollectionView for filtering, sorting, grouping
- Example: ListBox with ContentControl bound to SelectedItem

## 3. Dependency Properties

### Q7: What are Dependency Properties and why are they needed?
**Answer:**
- Special property type that supports WPF subsystems
- Benefits: data binding, styling, animation, property inheritance
- Memory efficient (stores only non-default values)
- Supports property value coercion and validation
- Enables change notification callbacks
- Required for custom controls and templating

### Q8: Explain property value precedence in WPF
**Answer:**
From highest to lowest priority:
1. Animations (active)
2. Local value (SetValue)
3. Triggers
4. Styles
5. Property inheritance
6. Default value (metadata)
- CoerceValueCallback can override
- Can use ClearValue to reset to inherited value

### Q9: What are Attached Properties and when would you use them?
**Answer:**
- Properties that can be attached to any DependencyObject
- Defined by one class, used on another
- RegisterAttached vs Register
- Require Get/Set accessor methods
- Use cases: Grid.Row, Canvas.Left, DockPanel.Dock
- Custom: adding behavior to controls you don't own

## 4. Commands

### Q10: Explain ICommand and how to implement it properly
**Answer:**
- Interface with Execute, CanExecute, CanExecuteChanged
- Decouples UI actions from business logic
- RelayCommand/DelegateCommand common implementations
- CanExecute disables controls automatically
- CommandManager.RequerySuggested for automatic updates
- CommandParameter passes data to command
- Supports keyboard gestures (RoutedUICommand)

### Q11: How do you handle async operations in Commands?
**Answer:**
- Use async Task methods in command Execute
- Can't directly await in ICommand.Execute (void)
- Pattern: async void Execute() wrapping async Task ExecuteAsync()
- Update CanExecute during operation
- Handle exceptions properly
- Consider AsyncRelayCommand from CommunityToolkit.Mvvm
- Update UI with progress/status

## 5. Templates & Styles

### Q12: What's the difference between ControlTemplate and DataTemplate?
**Answer:**
- **ControlTemplate**: Defines visual structure of control
- **DataTemplate**: Defines how data objects are displayed
- ControlTemplate uses TemplateBinding
- DataTemplate uses regular bindings to DataContext
- ControlTemplate in Control.Template
- DataTemplate in ItemsControl.ItemTemplate or ContentControl.ContentTemplate

### Q13: Explain Triggers and their types
**Answer:**
- **Property Trigger**: Based on property value
- **Data Trigger**: Based on bound data
- **Event Trigger**: Based on routed events
- **MultiTrigger**: Multiple conditions (AND)
- **MultiDataTrigger**: Multiple data conditions
- Can set properties or start animations
- Defined in Styles or ControlTemplates

### Q14: How do you implement custom controls vs user controls?
**Answer:**
- **UserControl**: Composition of existing controls, quick development
- **Custom Control**: Inherits from Control, full customization
- Custom controls use generic.xaml for default template
- UserControl defined in XAML with code-behind
- Custom control: better performance, templatable, reusable
- Use UserControl for app-specific UI, Custom Control for libraries

## 6. Routed Events

### Q15: Explain the three routing strategies in WPF
**Answer:**
- **Bubbling**: Event travels up visual tree (MouseDown, Click)
- **Tunneling**: Event travels down visual tree (Preview events)
- **Direct**: Only raised on source element
- Tunneling occurs before bubbling
- Can set e.Handled = true to stop routing
- RegisterRoutedEvent to create custom
- Preview events allow input validation/cancellation

### Q16: How does event bubbling help in WPF applications?
**Answer:**
- Reduces event handler count
- Handle events at parent level
- Simplifies complex UI scenarios
- Good for dynamic UI (TreeView, ListBox)
- Can filter by source element
- Example: Single Click handler on Panel for multiple buttons
- Performance benefit for large visual trees

## 7. Performance & Optimization

### Q17: What techniques do you use to optimize WPF performance?
**Answer:**
- Virtualization (VirtualizingStackPanel)
- Freeze Freezable objects (Brushes, Pens)
- Reduce visual tree depth
- Use simple templates
- Binding: OneTime when possible
- Async loading of data
- Background threads for heavy operations
- Measure with Visual Studio Profiler
- Enable hardware acceleration

### Q18: Explain UI Virtualization and when to use it
**Answer:**
- Creates UI elements only for visible items
- VirtualizingStackPanel (default in ListBox)
- Reduces memory and improves scrolling
- Important for large datasets (1000+ items)
- Can use recycling mode for better performance
- Disable for smooth scrolling animations
- Not suitable for complex item templates
- DataGrid uses virtualization by default

## 8. Threading & Dispatcher

### Q19: How do you handle threading in WPF applications?
**Answer:**
- UI thread (Dispatcher) owns all UI elements
- Use Dispatcher.Invoke/BeginInvoke for UI updates
- Modern: async/await automatically marshals to UI thread
- Task.Run for background work
- Avoid blocking UI thread
- Use BackgroundWorker for progress reporting
- Consider DispatcherPriority for operation ordering
- Be careful with data binding from background threads

### Q20: What is the Dispatcher and why is it important?
**Answer:**
- Manages work items queue for UI thread
- Ensures thread affinity for UI elements
- Single-threaded UI model for consistency
- Prevents cross-thread exceptions
- DispatcherPriority controls execution order
- Dispatcher.Invoke: synchronous, Dispatcher.BeginInvoke: asynchronous
- CheckAccess() to verify thread
- Modern async/await often eliminates need

## 9. Value Converters

### Q21: When and how do you use IValueConverter and IMultiValueConverter?
**Answer:**
- IValueConverter: Transform single value (bool to Visibility)
- IMultiValueConverter: Combine multiple bindings
- Convert: Source ? Target, ConvertBack: Target ? Source
- StringFormat can replace simple converters
- Cache converter instances in resources
- Return DependencyProperty.UnsetValue for invalid values
- Example: FullNameConverter combining FirstName + LastName

## 10. Advanced Topics

### Q22: Explain Freezable objects and their benefits
**Answer:**
- Base class for graphics objects (Brush, Pen, Transform)
- Freeze() makes immutable, thread-safe
- Frozen objects share memory across instances
- Better performance (no change notifications)
- Can cross thread boundaries
- Clone to make modifiable copy
- IsFrozen property to check state
- Use for better performance in templates

### Q23: How do you implement drag-and-drop in WPF?
**Answer:**
- Use DragDrop class (DoDragDrop)
- AllowDrop=true on target
- Handle PreviewMouseMove for drag start
- DragEnter, DragOver, Drop events
- Set DragEventArgs.Effects (Copy, Move, None)
- Create DataObject with data
- Query formats with GetDataPresent
- Visual feedback with DragAdorner

### Q24: What are Visual States and Visual State Manager?
**Answer:**
- Manages control states (Normal, MouseOver, Pressed)
- Defined in ControlTemplate
- Use GoToState to change states
- Transitions for smooth animations
- States grouped by StateName
- Alternative to Triggers for complex scenarios
- Better for Blend integration
- Part of modern control development

---

## Behavioral & Architectural Questions

### Q25: How would you architect a large enterprise WPF application?
**Answer:**
- MVVM with proper separation
- Dependency Injection (DI container)
- Navigation service for views
- Message/Event aggregator for loose coupling
- Repository pattern for data access
- Use Prism or similar framework
- Modular architecture
- Unit testing ViewModels
- Async/await throughout
- Logging and error handling
- Configuration management

### Q26: What testing strategies do you use for WPF applications?
**Answer:**
- Unit test ViewModels (no UI dependencies)
- Test Commands and INotifyPropertyChanged
- Mock services with interfaces
- Integration tests for Views
- UI automation tests (use FlaUI, WinAppDriver)
- Test converters and behaviors
- Test async operations
- Measure code coverage
- Use Test Driven Development (TDD)

---

## Common Pitfalls & Solutions

### Common Mistakes to Avoid:
1. Memory leaks from event handlers
2. Blocking UI thread
3. Not using virtualization for large lists
4. Overusing DynamicResource
5. Too much logic in code-behind
6. Not implementing IDisposable properly
7. Incorrect use of Dispatcher
8. Not handling async exceptions
9. Breaking MVVM with View references in ViewModel
10. Not freezing Freezable objects

### Performance Red Flags:
- No virtualization on large ItemsControls
- Complex visual trees
- Synchronous I/O on UI thread
- Too many bindings
- Not using compiled bindings (x:Bind in UWP)
- Reflection in converters
- Large images not optimized

---

This covers the essential WPF concepts expected from a 13-year experienced architect. Master these, and you'll be well-prepared for senior-level WPF interviews!
