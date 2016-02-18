using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class CsvRow {
    private List<string> data = null;
    private int index = 0;

    public CsvRow(List<string> data) {
        this.data = data;
    }

    public int Count {
        get { return data.Count; }
    }

    // random access apis
    //-------------------------------------------------------------------------
    public string AsString(int column) {
        return data[column];
    }

    public int AsInt(int column) {
        return Convert.ToInt32(data[column]);
    }

    public float AsFloat(int column) {
        return Convert.ToSingle(data[column]);
    }

    public bool AsBool(int column) {
        return data[column] == "TRUE";
    }

    public T AsEnum<T>(int column) where T : struct {
        return (T)Enum.Parse(typeof(T), AsString(column), true);
    }


    // next series apis
    //-------------------------------------------------------------------------
    public string NextString() {
        return data[index++];
    }

    public int NextInt() {
        return Convert.ToInt32(data[index++]);
    }

    public float NextFloat() {
        return Convert.ToSingle(data[index++]);
    }

    public bool NextBool() {
        string text = data[index++];
        return text == "TRUE" || text == "true";
    }

    public T NextEnum<T>() where T : struct {
        return (T)Enum.Parse(typeof(T), NextString(), true);
    }

    public bool HasNext() {
        return index < data.Count;
    }
}

public class CsvParser {
    private char[] delimiters = new char[] {'\r', '\n'};
    private List<CsvRow> data = new List<CsvRow>();

    public void Parse(string text, char delimiter = ',') {
        Parse(text, delimiter.ToString());
    }

    public void Parse(string text, string delimiter, StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries) {
        string[] lines = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        char [] columnDelimiters = new char[delimiter.Length];
        for (int i=0; i<delimiter.Length; i++) {
            columnDelimiters[i] = delimiter[i];
        }

        data.Clear();
        foreach (string line in lines) {
            if (string.IsNullOrEmpty(line) == true) {
                continue;
            }

            if (line[0] == '#') {
                continue;
            }

            string[] token = null; 
            token = line.Split(columnDelimiters, option);
            List<string> parts = new List<string>();
            foreach(string word in token) {
                parts.Add(word);
            }

            data.Add(new CsvRow(parts));
        }
    }

    public int Count {
        get { return data.Count; }
    }

    public CsvRow GetRow(int index) {
        return data[index];
    }
}