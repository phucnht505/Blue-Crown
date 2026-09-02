import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from '../../shared/header/header';
import { Footer } from '../../shared/footer/footer';
import { AiAssistant } from '../../shared/ai-assistant/ai-assistant';

@Component({
  selector: 'app-public-layout',
  standalone: true,
  imports: [RouterOutlet, Header, Footer, AiAssistant],
  templateUrl: './public-layout.html',
  styleUrl: './public-layout.css'
})
export class PublicLayout { }
