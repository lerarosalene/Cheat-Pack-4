using System.Collections;

internal class PatchedDeathEnumerator : IEnumerable
{
    public IEnumerator original;
    public bool shouldCheatMoney;
    public bool shouldCheatSpool;
    public PlayerData pd;

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
        var originalMoney = pd.geo;
        var originalSpoolState = pd.IsSilkSpoolBroken;

        while (original.MoveNext())
        {
            var item = original.Current;
            yield return item;
        }

        if (shouldCheatMoney)
        {
            pd.geo = originalMoney;
            pd.HeroCorpseMoneyPool = 0;
        }

        if (shouldCheatSpool)
        {
            pd.IsSilkSpoolBroken = originalSpoolState;
        }
    }
}
