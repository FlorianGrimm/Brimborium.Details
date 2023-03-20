namespace Brimborium.Details.Utility;

public static class OrderedItem {
    public static List<OrderedItem<T>> CreateList<T>(IEnumerable<T> items, Func<T, int> orderSelector) {
        var result = new List<OrderedItem<T>>();
        foreach (var item in items) {
            result.Add(new OrderedItem<T>(orderSelector(item), item));
        }
        result.Sort();
        return result;
    }
    public static int ExtractOrderFromComment(StringSlice comment) {
        comment = comment.Trim();
        if (comment.IsNullOrEmpty()) {
            return 0;
        }
        var numberText = comment.ReadWhile(
            (value, idx) => {
                return char.IsDigit(value)
                    || (idx == 0 && (value == '-' || value == '+'));
            });
        if (numberText.Length == 0) {
            return 0;
        } else {
            return int.TryParse(numberText.AsSpan(), out var result) ? result : 0;
        }
    }
}
public class OrderedItem<T> : IComparable<OrderedItem<T>> {
    public OrderedItem(int order, T item) {
        this.Order = order;
        this.Item = item;
    }

    public void Deconstruct(out int order, out T item) {
        order = this.Order;
        item = this.Item;
    }

    public int Order { get; }

    public T Item { get; }

    public int CompareTo(OrderedItem<T>? other) {
        if (other is null) { return 1; }
        return this.Order.CompareTo(other.Order);
    }
}