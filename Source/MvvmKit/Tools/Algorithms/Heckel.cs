using MvvmKit.HeckelDiff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MvvmKit.HeckelDiff.Entries;
using static MvvmKit.HeckelDiff.Operations;

namespace MvvmKit
{
    public static class Heckel
    {
        public static IEnumerable<Operation<T>> Diff<T>(IEnumerable<T> oldItems, IEnumerable<T> newItems)
            where T: IEquatable<T>
        {
            var old = oldItems.ToList();
            var @new = newItems.ToList();

            var table = new Dictionary<T, SymbolEntry<T>>();               // var table = [Int: SymbolEntry]()
            var oldArray = new List<Entry>();                           // var oa = [Entry]()
            var newArray = new List<Entry>();                           // var na = [Entry]()

            // Pass 1 comprises the following: (a) each line i of file N is read in sequence; (b) a symbol
            // table entry for each line i is created if it does not already exist; (c) NC for the line's
            // symbol table entry is incremented; and (d) NA [i] is set to point to the symbol table entry of

            // line i.
            foreach (var item in @new)                               // for item in new {
            {
                var entry = table.GetOrCreate(item, () => Symbol(item));    // let entry = table[item.hashValue] ?? SymbolEntry()
                                                                        // table[item.hashValue] = entry
                entry.NewCounter = entry.NewCounter.Increment();        // entry.nc.increment()
                newArray.Add(entry);                                    // na.append(.symbol(entry))
            }

            // Pass 2 is identical to pass 1 except that it acts on file O, array OA, and counter OC,
            // and OLNO for the symbol table entry is set to the line's number.

            foreach (var (index, item) in old.Enumerated())         // for (index, item) in old.enumerated()
            {
                var entry = table.GetOrCreate(item, () => Symbol(item));    // let entry = table[item.hashValue] ?? SymbolEntry()
                                                                        // table[item.hashValue] = entry
                entry.OldCounter = entry.OldCounter.Increment();        // entry.oc.increment()
                entry.OldNumbers.Add(index);                            // entry.olno.append(index)
                oldArray.Add(entry);                                    // oa.append(.symbol(entry))
            }

            // In pass 3 we use observation 1 and process only those lines having NC = OC = 1. Since each
            // represents (we assume) the same unmodified line, for each we replace the symbol table pointers
            // in NA and OA by the number of the line in the other file. For example, if NA[i] corresponds to
            // such a line, we look NA[i] up in the symbol table and set NA[i] to OLNO and OA[OLNO] to i.
            // In pass 3 we also "find" unique virtual lines immediately before the first and immediately
            // after the last lines of the files.

            foreach (var (index, item) in newArray.Enumerated())        // for (index, item) in na.enumerated() {
            {
                if ((item is SymbolEntry<T> entry)                         // if case let .symbol(entry) = item,                     
                    && entry.OccursInBoth                               // entry.occursInBoth, 
                    && entry.OldNumbers.Any())                          // !entry.olno.isEmpty {
                {
                    var oldIndex = entry.OldNumbers.First();            // let oldIndex = entry.olno.removeFirst()
                    entry.OldNumbers.RemoveAt(0);                       

                    newArray[index] = Index(oldIndex);                  // na[index] = .index(oldIndex)
                    oldArray[oldIndex] = Index(index);                  // oa[oldIndex] = .index(index)
                }
            }

            // In pass 4, we apply observation 2 and process each line in NA in ascending order: If NA[i]
            // points to OA[j] and NA[i + 1] and OA[j + 1] contain identical symbol table entry pointers, then
            // OA[j + 1] is set to line i + 1 and NA[i + 1] is set to line j + 1.

            var i = 1;                                                  // var i = 1
            while (i < newArray.Count - 1)                              // while i < na.count - 1 {
            {
                if ((newArray[i] is IndexEntry j)                       // if case let .index(j) = na[i]
                    && ((j.Index + 1) < oldArray.Count)                 //      , j + 1 < oa.count,
                    && (newArray[i + 1] is SymbolEntry<T> newEntry)        //      case let .symbol(newEntry) = na[i + 1],
                    && (oldArray[j.Index + 1] is SymbolEntry<T> oldEntry)  //      case let .symbol(oldEntry) = oa[j + 1], 
                    && (oldEntry == newEntry))                          //      newEntry === oldEntry {
                {
                    newArray[i + 1] = Index(j.Index + 1);               //      na[i + 1] = .index(j + 1)
                    oldArray[j.Index + 1] = Index(i + 1);               //      oa[j + 1] = .index(i + 1)
                }                                                       // }
                                        
                i++;                                                    // i += 1
            }


            // In pass 5, we also apply observation 2 and process each entry in descending order: if NA[i]
            // points to OA[j] and NA[i - 1] and OA[j - 1] contain identical symbol table pointers, then
            // NA[i - 1] is replaced by j - 1 and OA[j - 1] is replaced by i - 1.

            i = newArray.Count - 1;                                     // i = na.count - 1
            while (i > 0)                                               // while i > 0 {
            {
                if ((newArray[i] is IndexEntry j)                       //  if case let .index(j) = na[i]
                    && (j.Index -1 >= 0)                                //      , j - 1 >= 0,
                    && (newArray[i - 1] is SymbolEntry<T> newEntry)        //      case let .symbol(newEntry) = na[i - 1],
                    && (oldArray[j.Index - 1] is SymbolEntry<T> oldEntry)  //      case let .symbol(oldEntry) = oa[j - 1],
                    && (newEntry == oldEntry))                          //      newEntry === oldEntry {
                {
                    newArray[i - 1] = Index(j.Index - 1);               //      na[i - 1] = .index(j - 1)
                    oldArray[j.Index - 1] = Index(i - 1);               //      oa[j - 1] = .index(i - 1)
                }                                                       //      }

                i--;                                                    //  i -= 1
            }                                                           // }


            // yield return replaces the need to hold and a list        // var steps = [Operation]()
            var deleteOffsets = Enumerable.Repeat(0, old.Count)     // var deleteOffsets = Array(repeating: 0, count: old.count)
                                          .ToArray();
            var runningOffset = 0;                                      // var runningOffset = 0

            foreach (var (index, item) in oldArray.Enumerated())        // for (index, item) in oa.enumerated() {
            {
                deleteOffsets[index] = runningOffset;                   // deleteOffsets[index] = runningOffset
                if (item is SymbolEntry<T> se)                                // if case .symbol = item {
                {
                    yield return Delete(index, se.OldItem);             // steps.append(.delete(index))
                    runningOffset += 1;                                 // runningOffset += 1
                }
            }

            runningOffset = 0;                                          // runningOffset = 0

            foreach (var (index, item) in newArray.Enumerated())        // for (index, item) in na.enumerated() {
            {
                switch (item)                                           // switch item {
                {
                    case SymbolEntry<T> se:                             //  case .symbol:
                        yield return Insert(index, se.OldItem);         //      steps.append(.insert(index))
                        runningOffset += 1;                             //      runningOffset += 1
                        break;
                    case IndexEntry oldIndex:                           // case let .index(oldIndex):
                        // The object has changed, so it should be updated.
                        if (!Equals(old[oldIndex.Index], @new[index]))       // if old[oldIndex] != new[index] {
                        {                   
                            yield return Update(index, old[oldIndex.Index], @new[index]); // steps.append(.update(index))
                        }                                                           // }

                        var deleteOffset = deleteOffsets[oldIndex.Index];           // let deleteOffset = deleteOffsets[oldIndex]
                        // The object is not at the expected position, so move it.

                        var oldCalculatedIndex = oldIndex.Index - deleteOffset + runningOffset;
                        if (oldCalculatedIndex != index)   // if (oldIndex - deleteOffset + runningOffset) != index {
                        {
                            yield return Move(oldIndex.Index, index, old[oldIndex.Index]);     // steps.append(.move(oldIndex, index))
                        }
                        break;
                }
            }

        }

        public static IEnumerable<Operation<T>> OrderedDiff<T>(IEnumerable<T> oldItems, IEnumerable<T> newItems)
            where T : IEquatable<T>
        {
            var steps = Diff(oldItems, newItems).ToList();

            var old = oldItems.ToList();
            var @new = newItems.ToList();

            var insertions = new List<Operation<T>>();
            var updates = new List<Operation<T>>();
            var possibleDeletions = Enumerable.Repeat<Operation<T>>(null, old.Count).ToList();

            Action<int, Operation<T>> trackDeletion = (int fromIndex, Operation<T> step) =>
            {
                if (possibleDeletions[fromIndex] == null)
                    possibleDeletions[fromIndex] = step;
            };

            foreach (var step in steps)
            {
                switch (step)
                {
                    case InsertOp<T> ins:
                        insertions.Add(ins);
                        break;
                    case DeleteOp<T> del:
                        trackDeletion(del.Index, step);
                        break;
                    case MoveOp<T> mov:
                        insertions.Add(Insert(mov.To, mov.Item));
                        trackDeletion(mov.From, Delete(mov.From, mov.Item));
                        break;
                    case UpdateOp<T> up:
                        updates.Add(step);
                        break;
                }
            }

            var deletions = possibleDeletions.Where(x => x != null).Reverse();
            return deletions.Concat(insertions).Concat(updates);
        }
    }
}
