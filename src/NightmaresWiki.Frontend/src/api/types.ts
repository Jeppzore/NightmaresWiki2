export type EntryType = "enemy" | "item" | "npc";

export interface EntrySummary {
  type: EntryType;
  slug: string;
  title: string;
  summary: string;
  taxonomy: string[];
  image?: string;
  isDetailPage: boolean;
}

export interface WikiStat {
  label: string;
  value: string;
}

export interface BodySection {
  heading: string;
  content: string;
  items: string[];
}

export interface EntryRelationship {
  type: string;
  title: string;
  slug: string;
  image?: string;
}

export interface EntryDetail extends EntrySummary {
  bodySections: BodySection[];
  stats: WikiStat[];
  relationships: EntryRelationship[];
  sourcePath: string;
}

export interface HomeSection {
  type: EntryType;
  title: string;
  route: string;
  description: string;
  count: number;
  image?: string;
  statusNote?: string;
}

export interface HomeResponse {
  title: string;
  intro: string;
  sections: HomeSection[];
  highlights: EntrySummary[];
}
