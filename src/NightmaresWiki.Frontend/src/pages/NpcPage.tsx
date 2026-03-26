import { useEffect, useState } from "react";
import { getEntries } from "../api/client";
import type { EntrySummary } from "../api/types";
import { ErrorBlock } from "../components/ErrorBlock";
import { LoadingBlock } from "../components/LoadingBlock";

export function NpcPage() {
  const [entries, setEntries] = useState<EntrySummary[]>([]);
  const [error, setError] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    getEntries("npc")
      .then((payload) => {
        setEntries(payload);
        setError(false);
      })
      .catch(() => setError(true))
      .finally(() => setIsLoading(false));
  }, []);

  if (error) {
    return <ErrorBlock />;
  }

  if (isLoading) {
    return <LoadingBlock />;
  }

  const metadata = entries[0] ?? {
    type: "npc",
    slug: "npcs",
    title: "NPC",
    summary: "No dedicated NPC detail pages exist in the imported legacy source yet.",
    taxonomy: ["NPC"],
    isDetailPage: false,
  };

  return (
    <div className="page">
      <section className="panel">
        <p className="section-heading__eyebrow">Sparse Section</p>
        <h1>NPC</h1>
        <p>{metadata.summary}</p>
      </section>

      <section className="panel panel--muted">
        <h2>No dedicated NPC detail pages yet</h2>
        <p>
          The imported legacy repository only contains a top-level NPC landing page. This section is intentionally present in the
          information architecture so the frontend, API, and agent workflow already support future NPC expansion.
        </p>
      </section>
    </div>
  );
}
