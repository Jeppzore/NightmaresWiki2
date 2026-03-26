import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getHome, resolveMediaUrl } from "../api/client";
import type { HomeResponse } from "../api/types";
import { EntryCard } from "../components/EntryCard";
import { ErrorBlock } from "../components/ErrorBlock";
import { LoadingBlock } from "../components/LoadingBlock";

export function HomePage() {
  const [data, setData] = useState<HomeResponse | null>(null);
  const [error, setError] = useState(false);

  useEffect(() => {
    getHome()
      .then(setData)
      .catch(() => setError(true));
  }, []);

  if (error) {
    return <ErrorBlock />;
  }

  if (!data) {
    return <LoadingBlock />;
  }

  return (
    <div className="page">
      <section className="hero">
        <div className="hero__copy">
          <p className="hero__eyebrow">Imported Lore & Data</p>
          <h1>{data.title}</h1>
          <p>{data.intro}</p>
        </div>
        <div className="hero__sections">
          {data.sections.map((section) => (
            <Link key={section.type} to={section.route} className="topic-tile">
              {section.image ? <img src={resolveMediaUrl(section.image)} alt={section.title} /> : null}
              <div>
                <p>{section.title}</p>
                <strong>{section.count}</strong>
                <span>{section.description}</span>
                {section.statusNote ? <small>{section.statusNote}</small> : null}
              </div>
            </Link>
          ))}
        </div>
      </section>

      <section className="panel">
        <div className="section-heading">
          <div>
            <p className="section-heading__eyebrow">Highlights</p>
            <h2>Featured articles</h2>
          </div>
        </div>

        <div className="entry-grid">
          {data.highlights.map((entry) => (
            <EntryCard key={`${entry.type}-${entry.slug}`} entry={entry} />
          ))}
        </div>
      </section>
    </div>
  );
}
