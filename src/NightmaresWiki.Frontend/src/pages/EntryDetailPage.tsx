import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getEntry, resolveMediaUrl } from "../api/client";
import type { EntryDetail } from "../api/types";
import { ErrorBlock } from "../components/ErrorBlock";
import { LoadingBlock } from "../components/LoadingBlock";

export function EntryDetailPage({ fallbackType }: { fallbackType: "enemy" | "item" }) {
  const { slug } = useParams();
  const [entry, setEntry] = useState<EntryDetail | null>(null);
  const [error, setError] = useState(false);

  useEffect(() => {
    if (!slug) {
      return;
    }

    getEntry(slug)
      .then((payload) => {
        setEntry(payload);
        setError(false);
      })
      .catch(() => setError(true));
  }, [slug]);

  if (error) {
    return <ErrorBlock label="The requested article could not be found." />;
  }

  if (!entry) {
    return <LoadingBlock />;
  }

  return (
    <div className="page">
      <nav className="breadcrumb" aria-label="Breadcrumb">
        <Link to="/">Home</Link>
        <span>/</span>
        <Link to={fallbackType === "enemy" ? "/enemies" : "/items"}>
          {fallbackType === "enemy" ? "Enemies" : "Items"}
        </Link>
        <span>/</span>
        <strong>{entry.title}</strong>
      </nav>

      <article className="article-layout">
        <section className="article-main panel">
          <header className="article-header">
            <div>
              <p className="section-heading__eyebrow">{entry.type}</p>
              <h1>{entry.title}</h1>
              <p>{entry.summary}</p>
              <div className="taxonomy-list taxonomy-list--inline">
                {entry.taxonomy.map((item) => (
                  <span key={item}>{item}</span>
                ))}
              </div>
            </div>
          </header>

          {entry.bodySections.map((section) => (
            <section key={section.heading} className="article-section">
              <h2>{section.heading}</h2>
              {section.content ? <p>{section.content}</p> : null}
              {section.items.length > 0 ? (
                <ul className="article-list">
                  {section.items.map((item) => (
                    <li key={item}>{item}</li>
                  ))}
                </ul>
              ) : null}
            </section>
          ))}
        </section>

        <aside className="article-side">
          <section className="panel infobox">
            <div className="infobox__title">{entry.title}</div>
            {entry.image ? (
              <div className="infobox__media">
                <img className="article-header__image" src={resolveMediaUrl(entry.image)} alt={entry.title} />
              </div>
            ) : null}
            <div className="infobox__section">
              <h2>Classification</h2>
              <div className="taxonomy-list">
                {entry.taxonomy.map((item) => (
                  <span key={item}>{item}</span>
                ))}
              </div>
            </div>
          </section>

          {entry.stats.length > 0 ? (
            <section className="panel infobox infobox--stats">
              <div className="infobox__title">Properties</div>
              <dl className="stat-list">
                {entry.stats.map((stat) => (
                  <div key={stat.label}>
                    <dt>{stat.label}</dt>
                    <dd>{stat.value}</dd>
                  </div>
                ))}
              </dl>
            </section>
          ) : null}

          {entry.relationships.length > 0 ? (
            <section className="panel">
              <h2>Related</h2>
              <ul className="article-list">
                {entry.relationships.map((relationship) => (
                  <li key={`${relationship.type}-${relationship.slug}`}>
                    <Link to={relationshipPath(relationship.type, relationship.slug)}>
                      {relationship.title}
                    </Link>
                  </li>
                ))}
              </ul>
            </section>
          ) : null}
        </aside>
      </article>
    </div>
  );
}

function relationshipPath(type: string, slug: string) {
  if (type === "droppedBy") {
    return `/enemies/${slug}`;
  }

  if (type === "drops") {
    return `/items/${slug}`;
  }

  return `/items/${slug}`;
}
