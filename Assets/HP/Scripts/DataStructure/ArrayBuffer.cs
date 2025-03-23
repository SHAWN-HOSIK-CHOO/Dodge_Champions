using System.Collections.Generic;
using UnityEngine;

public class ArrayBuffer<T>
{
    public T[] _buffer { get; private set; }
    public int _index { get; private set; }
    public int _capacity { get; private set; }


    public T Get()
    {
        return _buffer[_index];
    }
    public void Set(int index, T val)
    {
        int newIndex = (index) % _capacity;
        _buffer[newIndex] = val;
    }

    public void Add(T val)
    {
        int newIndex = (_index + 1) % _capacity;
        _buffer[newIndex] = val;
        _index = newIndex;
    }

    public ArrayBuffer(int capacity)
    {
        _capacity = capacity;
        _buffer = new T[capacity];
        _index = 0;
    }

}
