using System;
using System.Collections.Generic;
using System.Linq;

namespace NapilnikTasks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Good iPhone12 = new Good("iPhone 12");
            Good iPhone11 = new Good("iPhone 11");

            Warehouse warehouse = new Warehouse(20);

            Shop shop = new Shop(warehouse);

            warehouse.Delivery(iPhone12, 10);
            warehouse.Delivery(iPhone11, 1);

            shop.ShowAvailableGoods();

            Cart cart = new Cart(shop);
            cart.Add(iPhone12, 4);

            cart.ShowRequestGoods();
            cart.MakeOrder();

            shop.ShowAvailableGoods();
        }
    }

    public class Shop
    {
        private readonly Warehouse _warehouse;

        public Shop(Warehouse warehouse)
        {
            _warehouse = warehouse;
        }

        public void ShowAvailableGoods()
        {
            foreach (var cell in _warehouse.Cells)
            {
                Console.WriteLine($"{cell.Good.Title}/{cell.Count}");
            }
        }

        public bool TryGetGood(Cell cell)
        {
            bool isExist = false;

            foreach (var warehouseCell in _warehouse.Cells)
            {
                if (cell.Good.Title == warehouseCell.Good.Title)
                {
                    if (cell.Count <= warehouseCell.Count)
                    {
                        isExist = true;
                    }
                }
            }

            return isExist;
        }

        public void Remove(Good good, int count)
        {
            _warehouse.Unloading(good, count);
        }
    }

    public class Warehouse
    {
        private readonly List<Cell> _cells;
        private readonly int _maxCapacity;

        public Warehouse(int maxCapacity)
        {
            if (maxCapacity <= 0) throw new ArgumentOutOfRangeException(nameof(maxCapacity));

            _cells = new List<Cell>();
            _maxCapacity = maxCapacity;
        }

        public IReadOnlyList<IReadOnlyCell> Cells => _cells;
        private int CurrentCapacity => _cells.Sum(cell => cell.Count);

        public void Delivery(Good good, int amount)
        {
            Cell newCell = new Cell(good, amount);

            if (CurrentCapacity + newCell.Count > _maxCapacity)
                throw new InvalidOperationException();

            int cellIndex = GetIndex(good);

            if (cellIndex == -1)
                _cells.Add(newCell);
            else
                _cells[cellIndex].Merge(newCell);
        }

        public void Unloading(Good good, int count)
        {
            int cellIndex = GetIndex(good);

            if (cellIndex == -1)
                throw new InvalidOperationException();

            _cells[cellIndex] = _cells[cellIndex].Remove(count);
        }

        private int GetIndex(Good good)
        {
            return _cells.FindIndex(x => x.Good.Title == good?.Title);
        }
    }

    public class Cart
    {
        private readonly List<Cell> _cells;
        private readonly Shop _shop;

        public Cart(Shop shop)
        {
            _cells = new List<Cell>();
            _shop = shop;
        }

        public void Add(Good title, int amount)
        {
            var newGood = new Cell(title, amount);

            if (_shop.TryGetGood(newGood) == false)
                throw new InvalidOperationException();

            _cells.Add(newGood);
        }

        public void ShowRequestGoods()
        {
            foreach (var cell in _cells)
            {
                Console.WriteLine($"{cell.Good.Title}/{cell.Count}");
            }
        }

        public void MakeOrder()
        {
            foreach (var cell in _cells)
            {
                if (_shop.TryGetGood(cell))
                {
                    _shop.Remove(cell.Good, cell.Count);
                }
            }
        }
    }

    public class Good
    {
        public Good(string title)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
        }

        public string Title { get; }
    }

    public class Cell : IReadOnlyCell
    {
        public Cell(Good good, int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
            Good = good ?? throw new ArgumentNullException(nameof(good));
            Count = count;
        }

        public Good Good { get; }
        public int Count { get; private set; }

        public void Merge(Cell newCell)
        {
            if (newCell.Good != Good)
                throw new InvalidOperationException();

            Count += newCell.Count;
        }

        public Cell Remove(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (CanRemove(count) == false)
                throw new InvalidOperationException();

            return new Cell(Good, Count - count);
        }

        private bool CanRemove(int count)
        {
            return count <= Count;
        }
    }

    public interface IReadOnlyCell
    {
        public Good Good { get; }
        public int Count { get; }
    }
}