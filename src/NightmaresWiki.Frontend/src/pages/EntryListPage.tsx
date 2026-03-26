import { useEffect, useMemo, useState } from "react";
import { getEntries } from "../api/client";
import type { EntrySummary } from "../api/types";
import { EntryCard } from "../components/EntryCard";
import { ErrorBlock } from "../components/ErrorBlock";
import { LoadingBlock } from "../components/LoadingBlock";

interface EntryListPageProps {
  type: "enemy" | "item";
  title: string;
}

export function EntryListPage({ type, title }: EntryListPageProps) {
  const [entries, setEntries] = useState<EntrySummary[]>([]);
  const [activeCategory, setActiveCategory] = useState<string>("all");
  const [error, setError] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    setIsLoading(true);
    getEntries(type)
      .then((payload) => {
        setEntries(payload);
        setError(false);
      })
      .catch(() => setError(true))
      .finally(() => setIsLoading(false));
  }, [type]);

  const categories = useMemo(() => {
    const items = new Set<string>();
    entries.forEach((entry) => {
      entry.taxonomy.slice(1).forEach((taxonomy) => items.add(taxonomy));
    });

    return ["all", ...Array.from(items).sort((left, right) => left.localeCompare(right))];
  }, [entries]);

  const filteredEntries = useMemo(() => {
    if (activeCategory === "all") {
      return entries;
    }

    return entries.filter((entry) => entry.taxonomy.includes(activeCategory));
  }, [activeCategory, entries]);

  if (error) {
    return <ErrorBlock />;
  }

  if (isLoading) {
    return <LoadingBlock />;
  }

  return (
    <div className="page">
      <section className="panel">
        <div className="section-heading">
          <div>
            <p className="section-heading__eyebrow">Archive</p>
            <h1>{title}</h1>
            <p>{filteredEntries.length} article(s) available from the imported Nightmares source.</p>
          </div>
        </div>

        <div className="filter-row" aria-label={`${title} filters`}>
          {categories.map((category) => (
            <button
              key={category}
              className={category === activeCategory ? "filter-chip filter-chip--active" : "filter-chip"}
              onClick={() => setActiveCategory(category)}
              type="button"
            >
              {category}
            </button>
          ))}
        </div>
      </section>

      <section className="entry-grid" aria-label={`${title} entries`}>
        {filteredEntries.map((entry) => (
          <EntryCard key={entry.slug} entry={entry} />
        ))}
      </section>
    </div>
  );
}
