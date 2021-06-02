using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;

namespace Witness
{
    public interface ICell
    {
        int Value { get; }
        int X { get; }
        int Y { get; }
        Point Location { get; }
        IEnumerable<ICell> GetNeighbors();
    }

    public interface IVertex
    {
        int X { get; }
        int Y { get; }
        Point Location { get; }
    }

    public interface IPuzzle
    {
        Image<Rgba32> Image { get; }
        Rectangle BoundingBox { get; }
        void WriteToConsole();
        IEnumerable<ICell> Cells { get; }
        IEnumerable<IVertex> Solve();
    }

    public abstract class Puzzle<TData> : IPuzzle
    {
        private readonly List<Vertex> _path = new List<Vertex>();
        private readonly Cell[,] _cells;
        private readonly Vertex[,] _vertices;
        private readonly Vertex _origin;
        private readonly Vertex _target;
        private readonly int _gapSize;

        protected Puzzle(Region region, Image<Rgba32> image)
        {
            Image = image;
            GrayImage = image.Clone(i => i.Grayscale());
            BoundingBox = region.BoundingBox;
            CellSize = (BoundingBox.Right - BoundingBox.Left) / 6;
            _gapSize = (BoundingBox.Right - BoundingBox.Left - GridSize * CellSize) / 5;

            _cells = new Cell[GridSize, GridSize];
            PopulateCells();
            _vertices = new Vertex[GridSize + 1, GridSize + 1];
            PopulateVertices();

            _origin = _vertices[0, GridSize];
            _target = _vertices[GridSize, 0];
        }

        public Image<Rgba32> Image { get; private set; }
        public Rectangle BoundingBox { get; private set; }

        public void WriteToConsole()
        {
            Console.WriteLine($"{Description} puzzle found");
            for (int y = 0; y < GridSize; y++)
            {
                Console.Write(":");
                for (int x = 0; x < GridSize; x++)
                    Console.Write(_cells[x, y].Value + ":");
                Console.WriteLine();
            }
        }

        public IEnumerable<ICell> Cells
        {
            get
            {
                foreach (var c in _cells)
                    yield return c;
            }
        }

        public IEnumerable<IVertex> Solve()
        {
            _path.Add(_origin);
            if (!FindPathToTarget(_origin))
            {
                Console.WriteLine("Solution not found");
                return null;
            }
            Console.WriteLine("Length of path: " + _path.Count);
            return _path;
        }

        protected int GridSize => 4;
        protected Image<Rgba32> GrayImage { get; private set; }
        protected int CellSize { get; private set; }

        protected bool TryGetCell(int x, int y, out ICell cell)
        {
            var exists = x >= 0 && x < GridSize && y >= 0 && y < GridSize;
            cell = exists ? _cells[x, y] : null;
            return exists;
        }

        protected abstract string Description { get; }

        // Derived class should implement this to provide a meaningful integer value describing the content of
        // a cell, based on the pixel location provided (which is a best guess at the center point of the cell).
        protected abstract int GetCellValue(Point pixel);

        // Derived classes should use this to validate whether the rules for the cell have been broken or not.
        protected abstract bool ValidateCell(ICell cell);

        // Allows derived classes to do any extra processing they need for a proposed move.
        // The returned data will be passed back to the derived class via RemovingPathSegment should
        // the proposed move not result in a successful path through the puzzle.
        protected abstract TData AddingPathSegment(IVertex from, IVertex to);

        // Allows derived classes to undo any extra processing for a proposed move (see AddingPathSegment).
        protected abstract void RemovingPathSegment(TData data);

        private void PopulateCells()
        {
            int[] verticalCoords = GetCellMidpoints(BoundingBox.Top);
            int[] horizontalCoords = GetCellMidpoints(BoundingBox.Left);

            for (int i = 0; i < GridSize; i++)
                for (int j = 0; j < GridSize; j++)
                {
                    var location = new Point(horizontalCoords[i], verticalCoords[j]);
                    _cells[i, j] = new Cell(i, j, this)
                    {
                        Value = GetCellValue(location),
                        Location = location
                    };
                }
        }

        private void PopulateVertices()
        {
            for (int i = 0; i <= GridSize; i++)
                for (int j = 0; j <= GridSize; j++)
                    _vertices[i, j] = new Vertex(i, j, this)
                    {
                        Location = new Point(BoundingBox.Left + _gapSize / 2 + i * (CellSize + _gapSize), BoundingBox.Top + _gapSize / 2 + j * (CellSize + _gapSize))
                    };
        }

        private int[] GetCellMidpoints(int start)
        {
            var cellMidpoint = start + _gapSize + CellSize / 2;
            var coords = new int[GridSize];
            for (var i = 0; i < coords.Length; i++)
            {
                coords[i] = cellMidpoint;
                cellMidpoint = cellMidpoint + _gapSize + CellSize;
            }
            return coords;
        }

        private bool FindPathToTarget(Vertex from)
        {
            if (from == _target)
            {
                foreach (var cell in Cells)
                    if (!ValidateCell(cell))
                        return false;
                return true;
            }

            foreach (var to in from.GetUnvisitedNeighbors())
            {
                var data = AddingPathSegment(from, to);
                if (data == null) // No data returned means the derived class rejected this movement as a possibility
                    continue;

                _path.Add(to);
                if (FindPathToTarget(to))
                    return true;

                // No valid path found using this movement, so undo it and try the next one
                RemovingPathSegment(data);
                _path.RemoveAt(_path.Count - 1);
            }

            return false;
        }

        private bool TryGetVertex(int x, int y, out Vertex vertex)
        {
            var exists = x >= 0 && x <= GridSize && y >= 0 && y <= GridSize;
            vertex = exists ? _vertices[x, y] : null;
            return exists;
        }

        private class Vertex : GridElement, IVertex
        {
            public Vertex(int x, int y, Puzzle<TData> puzzle) : base(x, y, puzzle) { }

            public IEnumerable<Vertex> GetUnvisitedNeighbors()
            {
                foreach (var possibleNeighbor in Coords.GetNeighbors())
                    if (Puzzle.TryGetVertex(possibleNeighbor.X, possibleNeighbor.Y, out var vertex) && !Puzzle._path.Contains(vertex))
                        yield return vertex;
            }
        }

        private class Cell : GridElement, ICell
        {
            public Cell(int x, int y, Puzzle<TData> puzzle) : base(x, y, puzzle) { }

            public int Value { get; set; }

            public IEnumerable<ICell> GetNeighbors()
            {
                foreach (var possibleNeighbor in Coords.GetNeighbors())
                    if (Puzzle.TryGetCell(possibleNeighbor.X, possibleNeighbor.Y, out var cell))
                        yield return cell;
            }
        }

        private abstract class GridElement
        {
            protected GridElement(int x, int y, Puzzle<TData> puzzle)
            {
                Coords = new Point(x, y);
                Puzzle = puzzle;
            }

            public int X => Coords.X;
            public int Y => Coords.Y;
            public Point Location { get; set; }
            protected Point Coords { get; private set; }
            protected Puzzle<TData> Puzzle { get; private set; }
        }
    }
}
