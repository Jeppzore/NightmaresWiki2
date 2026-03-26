import type { EntryDetail, EntrySummary, HomeResponse } from "./types";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "";

async function getJson<T>(path: string): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`);

  if (!response.ok) {
    throw new Error(`Request failed: ${response.status}`);
  }

  return response.json() as Promise<T>;
}

export function getHome() {
  return getJson<HomeResponse>("/api/home");
}

export function getEntries(type: string, category?: string) {
  const params = new URLSearchParams({ type });
  if (category) {
    params.set("category", category);
  }

  return getJson<EntrySummary[]>(`/api/entries?${params.toString()}`);
}

export function getEntry(slug: string) {
  return getJson<EntryDetail>(`/api/entries/${slug}`);
}

export function searchEntries(query: string) {
  const params = new URLSearchParams({ q: query });
  return getJson<EntrySummary[]>(`/api/search?${params.toString()}`);
}

export function resolveMediaUrl(path?: string) {
  if (!path) {
    return "";
  }

  if (path.startsWith("http://") || path.startsWith("https://")) {
    return path;
  }

  return API_BASE_URL ? `${API_BASE_URL}${path}` : path;
}
