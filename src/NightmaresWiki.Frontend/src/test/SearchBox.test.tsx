import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { describe, expect, it, vi } from "vitest";
import { SearchBox } from "../components/SearchBox";

describe("SearchBox", () => {
  it("shows search suggestions", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        json: async () => [
          {
            type: "enemy",
            slug: "green-slime",
            title: "Green Slime",
            summary: "A green slime.",
            taxonomy: ["Enemies"],
            isDetailPage: true
          }
        ]
      }),
    );

    render(
      <MemoryRouter>
        <SearchBox />
      </MemoryRouter>,
    );

    fireEvent.change(screen.getByLabelText("Search entries"), { target: { value: "green" } });

    await waitFor(() => expect(screen.getByText("Green Slime")).toBeInTheDocument(), { timeout: 2000 });
  });
});
