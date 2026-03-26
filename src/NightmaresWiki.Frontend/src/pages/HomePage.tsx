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
      <section className="panel wiki-intro">
        <div className="wiki-intro__copy">
          <p className="hero__eyebrow">Imported Lore & Data</p>
          <h1>{data.title}</h1>
          <p>{data.intro}</p>
          <p className="wiki-intro__note">
            Browse the archive much like a creature compendium: compact sprites, quick categories, and article-first pages.
          </p>
        </div>
        <div className="wiki-intro__sections">
          {data.sections.map((section) => (
            <Link key={section.type} to={section.route} className="topic-tile">
              {section.image ? (
                <div className="topic-tile__media">
                  <img src={resolveMediaUrl(section.image)} alt={section.title} />
                </div>
              ) : null}
              <div className="topic-tile__body">
                <div className="topic-tile__header">
                  <strong>{section.title}</strong>
                  <span>{section.count}</span>
                </div>
                <p>{section.description}</p>
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
            <p>Selected entries from the imported Nightmares archive.</p>
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
