using System.Diagnostics;
using ChangeSetCalculation.Models;

namespace ChangeSetCalculation.Extensions;

internal static class EnumerableUpdateActionsExtensions
{
    extension<TSource, TTarget>(EnumerableChangeSet<TSource, TTarget> source)
    {
        public EnumerableChangeSet<TSource, TTarget> AddUpdateActions(
            IEnumerable<ChangeAction<TSource, TTarget>> actions) =>
            actions.Aggregate(source, AddUpdateAction);

        public EnumerableChangeSet<TSource, TTarget> AddUpdateAction(
            ChangeAction<TSource, TTarget> action) =>
            action switch
            {
                ChangeAction<TSource, TTarget>.Add addAction =>
                    source with
                    {
                        ToAdd = source.ToAdd.Concat(addAction.Values),
                    },
                ChangeAction<TSource, TTarget>.Update updateAction =>
                    source with
                    {
                        ToUpdate = source.ToUpdate.Concat(updateAction.Updates),
                    },
                ChangeAction<TSource, TTarget>.Delete deleteAction =>
                    source with
                    {
                        ToDelete = source.ToDelete.Concat(deleteAction.Keys),
                    },
                _ => throw new UnreachableException(),
            };

        public ChangeSet<TSource, TTarget> ToArrays() =>
            new()
            {
                ToAdd = source.ToAdd.ToArray(),
                ToUpdate = source.ToUpdate.ToArray(),
                ToDelete = source.ToDelete.ToArray(),
            };
    }
}
