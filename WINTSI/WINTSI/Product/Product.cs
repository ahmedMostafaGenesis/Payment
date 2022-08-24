internal class Product
{
    public string name;
    public float price;
    public int quantity;
    public float GetTotalPrice() => this.price * this.quantity;
}