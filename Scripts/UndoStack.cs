using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class UndoStack<T>
{
    [JsonProperty] private readonly List<T> _history = new List<T>();

    [JsonProperty] private int _currentIndex = -1;

    public UndoStack(T initialValue)
    {
        Push(initialValue);
    }

    public void Push(T value)
    {
        // Remove any redo history
        if (_currentIndex < _history.Count - 1)
        {
            _history.RemoveRange(_currentIndex + 1, _history.Count - _currentIndex - 1);
        }

        _history.Add(value);
        _currentIndex++;
    }

    public T Current(int indexOverwrite = -1)
    {
        if (indexOverwrite >= 0 && indexOverwrite < _history.Count)
        {
            return _history[indexOverwrite];
        }
        //else
        if (_currentIndex >= 0 && _currentIndex < _history.Count)
        {
            return _history[_currentIndex];
        }

        throw new InvalidOperationException("No current state available.");
    }

    public bool Undo()
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            return true;
        }

        return false;
    }

    public bool Redo()
    {
        if (_currentIndex < _history.Count - 1)
        {
            _currentIndex++;
            return true;
        }

        return false;
    }

    public IEnumerable<(T item, bool isCurrent)> GetFullHistory()
    {
        return _history.Select((item, index) => (item, index == _currentIndex));
    }
}